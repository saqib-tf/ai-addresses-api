using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using ai_addresses_api; // Added for Constants
using ai_addresses_data.Entity;
using ai_addresses_services.Commands;
using ai_addresses_services.Models;
using ai_addresses_services.Queries;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace ai_addresses_api
{
    public class PersonFunction
    {
        private readonly IMediator _mediator;

        public PersonFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetPersonById")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "person/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var person = await _mediator.Send(new GetPersonByIdQuery(id));
            return person is not null ? new OkObjectResult(person) : new NotFoundResult();
        }

        [Function("GetAllPersons")]
        public async Task<IActionResult> GetAllPersonsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "person")] HttpRequestData req
        )
        {
            var persons = await _mediator.Send(new GetAllPersonsQuery());
            return new OkObjectResult(persons);
        }

        [Function("CreatePerson")]
        public async Task<HttpResponseData> CreatePersonAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "person")]
                HttpRequestData req
        )
        {
            var data = await req.ReadFromJsonAsync<Person>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var command = new CreatePersonCommand(data);

            var person = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(person);
            return response;
        }

        [Function("UpdatePerson")]
        public async Task<HttpResponseData> UpdatePersonAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "person/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var data = await req.ReadFromJsonAsync<Person>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            data.Id = id;
            var command = new UpdatePersonCommand(data);

            var person = await _mediator.Send(command);

            if (person == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Person not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(person);
            return response;
        }

        [Function("DeletePerson")]
        public async Task<HttpResponseData> DeletePersonAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "person/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var result = await _mediator.Send(new DeletePersonCommand(id));
            var response = req.CreateResponse(
                result ? HttpStatusCode.NoContent : HttpStatusCode.NotFound
            );
            if (!result)
                await response.WriteStringAsync("Person not found.");
            return response;
        }

        [Function("SearchPersons")]
        public async Task<IActionResult> SearchPersonsAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "person/search")]
                HttpRequestData req
        )
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            var searchTerm = queryParams["searchTerm"];
            var sortBy = queryParams["sortBy"];
            var sortDescending = bool.TryParse(queryParams["sortDescending"], out var desc) && desc;
            var pageNumber = int.TryParse(queryParams["pageNumber"], out var pn) ? pn : 1;
            var pageSize = int.TryParse(queryParams["pageSize"], out var ps) ? ps : 10;

            var pagedQuery = new PagedQuery(
                searchTerm,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize
            );

            var result = await _mediator.Send(new SearchPersonQuery(pagedQuery));
            return new OkObjectResult(result);
        }

        [Function("UploadPersonImage")]
        public async Task<IActionResult> UploadPersonImageAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "person/upload-image")]
            //HttpRequestData req
            HttpRequest req
        )
        {
            var formdata = await req.ReadFormAsync();

            string name = formdata["name"];
            //string[] interests = JsonConvert.DeserializeObject<string[]>(formdata["interests"]);

            var image = req.Form.Files["file"];

            // do stuff with data.....
            if (image != null && image.Length > 0)
            {
                // Get the file extension
                var extension = Path.GetExtension(image.FileName);

                // Generate a new GUID filename with the original extension
                var newFileName = $"{Guid.NewGuid()}{extension}";

                // Ensure the images directory exists
                var imagesDir = Path.Combine(Directory.CreateTempSubdirectory().FullName);
                if (!Directory.Exists(imagesDir))
                    Directory.CreateDirectory(imagesDir);

                // Build the full file path
                var filePath = Path.Combine(imagesDir, newFileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Upload to Azure Blob Storage
                string connectionString = SecretUtility.AZURE_BLOB_CONNECTION_STRING ?? "";
                string containerName = Constants.ProfilePicturesContainer;

                var blobServiceClient = new BlobServiceClient(connectionString);
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blobClient = containerClient.GetBlobClient(newFileName);
                using (var fileStream = File.OpenRead(filePath))
                {
                    await blobClient.UploadAsync(fileStream, overwrite: true);
                }

                var blobUri = blobClient.Uri.ToString();

                return new OkObjectResult(
                    new { message = "File uploaded successfully", path = blobUri }
                );
            }

            return new BadRequestObjectResult("Sorry didn't get it all....");
        }
    }
}

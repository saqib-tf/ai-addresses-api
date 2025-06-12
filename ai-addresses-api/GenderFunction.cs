using System.Net;
using System.Threading.Tasks;
using ai_addresses_data.Entity;
using ai_addresses_services.Commands;
using ai_addresses_services.Models;
using ai_addresses_services.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ai_addresses_api
{
    public class GenderFunction
    {
        private readonly IMediator _mediator;

        public GenderFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetGenderById")]
        public async Task<IActionResult> GetGenderByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gender/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var gender = await _mediator.Send(new GetGenderByIdQuery(id));
            return gender is not null ? new OkObjectResult(gender) : new NotFoundResult();
        }

        [Function("GetAllGenders")]
        public async Task<IActionResult> GetAllGendersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gender")] HttpRequestData req
        )
        {
            var genders = await _mediator.Send(new GetAllGendersQuery());
            return new OkObjectResult(genders);
        }

        [Function("CreateGender")]
        public async Task<HttpResponseData> CreateGenderAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "gender")]
                HttpRequestData req
        )
        {
            var gender = await req.ReadFromJsonAsync<Gender>();
            if (gender == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var command = new CreateGenderCommand(gender);
            var createdGender = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(createdGender);
            return response;
        }

        [Function("UpdateGender")]
        public async Task<HttpResponseData> UpdateGenderAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "gender/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var gender = await req.ReadFromJsonAsync<Gender>();
            if (gender == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            gender.Id = id;
            var command = new UpdateGenderCommand(gender);
            var updatedGender = await _mediator.Send(command);

            if (updatedGender == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Gender not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(updatedGender);
            return response;
        }

        [Function("DeleteGender")]
        public async Task<HttpResponseData> DeleteGenderAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "gender/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var result = await _mediator.Send(new DeleteGenderCommand(id));
            var response = req.CreateResponse(
                result ? HttpStatusCode.NoContent : HttpStatusCode.NotFound
            );
            if (!result)
                await response.WriteStringAsync("Gender not found.");
            return response;
        }

        [Function("SearchGenders")]
        public async Task<IActionResult> SearchGendersAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "gender/search")]
                HttpRequestData req
        )
        {
            // Parse query parameters
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

            var result = await _mediator.Send(new SearchGenderQuery(pagedQuery));
            return new OkObjectResult(result);
        }
    }
}

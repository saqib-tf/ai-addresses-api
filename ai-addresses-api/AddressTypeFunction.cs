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
    public class AddressTypeFunction
    {
        private readonly IMediator _mediator;

        public AddressTypeFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetAddressTypeById")]
        public async Task<IActionResult> GetAddressTypeByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addresstype/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var addressType = await _mediator.Send(new GetAddressTypeByIdQuery(id));
            return addressType is not null ? new OkObjectResult(addressType) : new NotFoundResult();
        }

        [Function("GetAllAddressTypes")]
        public async Task<IActionResult> GetAllAddressTypesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addresstype")]
                HttpRequestData req
        )
        {
            var addressTypes = await _mediator.Send(new GetAllAddressTypesQuery());
            return new OkObjectResult(addressTypes);
        }

        [Function("CreateAddressType")]
        public async Task<HttpResponseData> CreateAddressTypeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "addresstype")]
                HttpRequestData req
        )
        {
            var data = await req.ReadFromJsonAsync<AddressType>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var command = new CreateAddressTypeCommand(data);
            var addressType = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(addressType);
            return response;
        }

        [Function("UpdateAddressType")]
        public async Task<HttpResponseData> UpdateAddressTypeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "addresstype/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var data = await req.ReadFromJsonAsync<AddressType>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            data.Id = id;
            var command = new UpdateAddressTypeCommand(data);
            var addressType = await _mediator.Send(command);

            if (addressType == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("AddressType not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(addressType);
            return response;
        }

        [Function("DeleteAddressType")]
        public async Task<HttpResponseData> DeleteAddressTypeAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "addresstype/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var result = await _mediator.Send(new DeleteAddressTypeCommand(id));
            var response = req.CreateResponse(
                result ? HttpStatusCode.NoContent : HttpStatusCode.NotFound
            );
            if (!result)
                await response.WriteStringAsync("AddressType not found.");
            return response;
        }

        [Function("SearchAddressTypes")]
        public async Task<IActionResult> SearchAddressTypesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "addresstype/search")]
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

            var result = await _mediator.Send(new SearchAddressTypeQuery(pagedQuery));
            return new OkObjectResult(result);
        }
    }
}

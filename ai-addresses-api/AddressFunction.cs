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
    public class AddressFunction
    {
        private readonly IMediator _mediator;

        public AddressFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetAddressById")]
        public async Task<IActionResult> GetAddressByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "address/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var address = await _mediator.Send(new GetAddressByIdQuery(id));
            return address is not null ? new OkObjectResult(address) : new NotFoundResult();
        }

        [Function("GetAllAddresses")]
        public async Task<IActionResult> GetAllAddressesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "address")]
                HttpRequestData req
        )
        {
            var addresses = await _mediator.Send(new GetAllAddressesQuery());
            return new OkObjectResult(addresses);
        }

        [Function("CreateAddress")]
        public async Task<HttpResponseData> CreateAddressAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "address")]
                HttpRequestData req
        )
        {
            var data = await req.ReadFromJsonAsync<Address>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var command = new CreateAddressCommand(data);
            var address = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(address);
            return response;
        }

        [Function("UpdateAddress")]
        public async Task<HttpResponseData> UpdateAddressAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "address/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var data = await req.ReadFromJsonAsync<Address>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            data.Id = id;
            var command = new UpdateAddressCommand(data);
            var address = await _mediator.Send(command);

            if (address == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Address not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(address);
            return response;
        }

        [Function("DeleteAddress")]
        public async Task<HttpResponseData> DeleteAddressAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "address/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var result = await _mediator.Send(new DeleteAddressCommand(id));
            var response = req.CreateResponse(
                result ? HttpStatusCode.NoContent : HttpStatusCode.NotFound
            );
            if (!result)
                await response.WriteStringAsync("Address not found.");
            return response;
        }

        [Function("SearchAddresses")]
        public async Task<IActionResult> SearchAddressesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "address/search")]
                HttpRequestData req
        )
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            var searchTerm = queryParams["searchTerm"];
            var sortBy = queryParams["sortBy"];
            var sortDescending = bool.TryParse(queryParams["sortDescending"], out var desc) && desc;
            var pageNumber = int.TryParse(queryParams["pageNumber"], out var pn) ? pn : 1;
            var pageSize = int.TryParse(queryParams["pageSize"], out var ps) ? ps : 10;
            var personId = int.TryParse(queryParams["personId"], out var pid) ? pid : (int?)null;

            var pagedQuery = new PagedQuery(
                searchTerm,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize
            );

            var result = await _mediator.Send(new SearchAddressQuery(pagedQuery, personId));
            return new OkObjectResult(result);
        }
    }
}

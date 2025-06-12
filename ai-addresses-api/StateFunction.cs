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
    public class StateFunction
    {
        private readonly IMediator _mediator;

        public StateFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetStateById")]
        public async Task<IActionResult> GetStateByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "state/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var state = await _mediator.Send(new GetStateByIdQuery(id));
            return state is not null ? new OkObjectResult(state) : new NotFoundResult();
        }

        [Function("GetAllStates")]
        public async Task<IActionResult> GetAllStatesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "state")] HttpRequestData req
        )
        {
            var states = await _mediator.Send(new GetAllStatesQuery());
            return new OkObjectResult(states);
        }

        [Function("CreateState")]
        public async Task<HttpResponseData> CreateStateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "state")] HttpRequestData req
        )
        {
            var data = await req.ReadFromJsonAsync<State>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var command = new CreateStateCommand(data);
            var state = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(state);
            return response;
        }

        [Function("UpdateState")]
        public async Task<HttpResponseData> UpdateStateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "state/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var data = await req.ReadFromJsonAsync<State>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            data.Id = id;
            var command = new UpdateStateCommand(data);
            var state = await _mediator.Send(command);

            if (state == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("State not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(state);
            return response;
        }

        [Function("DeleteState")]
        public async Task<HttpResponseData> DeleteStateAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "state/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var result = await _mediator.Send(new DeleteStateCommand(id));
            var response = req.CreateResponse(
                result ? HttpStatusCode.NoContent : HttpStatusCode.NotFound
            );
            if (!result)
                await response.WriteStringAsync("State not found.");
            return response;
        }

        [Function("SearchStates")]
        public async Task<IActionResult> SearchStatesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "state/search")]
                HttpRequestData req
        )
        {
            var queryParams = System.Web.HttpUtility.ParseQueryString(req.Url.Query);

            var searchTerm = queryParams["searchTerm"];
            var sortBy = queryParams["sortBy"];
            var sortDescending = bool.TryParse(queryParams["sortDescending"], out var desc) && desc;
            var pageNumber = int.TryParse(queryParams["pageNumber"], out var pn) ? pn : 1;
            var pageSize = int.TryParse(queryParams["pageSize"], out var ps) ? ps : 10;
            var countryId = int.TryParse(queryParams["countryId"], out var cid) ? cid : (int?)null;

            var pagedQuery = new PagedQuery(
                searchTerm,
                sortBy,
                sortDescending,
                pageNumber,
                pageSize
            );

            var result = await _mediator.Send(new SearchStateQuery(pagedQuery, countryId));
            return new OkObjectResult(result);
        }
    }
}

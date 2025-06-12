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
    public class CountryFunction
    {
        private readonly IMediator _mediator;

        public CountryFunction(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Function("GetCountryById")]
        public async Task<IActionResult> GetCountryByIdAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "country/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var country = await _mediator.Send(new GetCountryByIdQuery(id));
            return country is not null ? new OkObjectResult(country) : new NotFoundResult();
        }

        [Function("GetAllCountries")]
        public async Task<IActionResult> GetAllCountriesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "country")]
                HttpRequestData req
        )
        {
            var countries = await _mediator.Send(new GetAllCountriesQuery());
            return new OkObjectResult(countries);
        }

        [Function("CreateCountry")]
        public async Task<HttpResponseData> CreateCountryAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "country")]
                HttpRequestData req
        )
        {
            var data = await req.ReadFromJsonAsync<Country>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            var command = new CreateCountryCommand(data);
            var country = await _mediator.Send(command);

            var response = req.CreateResponse(HttpStatusCode.Created);
            await response.WriteAsJsonAsync(country);
            return response;
        }

        [Function("UpdateCountry")]
        public async Task<HttpResponseData> UpdateCountryAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "country/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var data = await req.ReadFromJsonAsync<Country>();
            if (data == null)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid request payload.");
                return badResponse;
            }

            data.Id = id;
            var command = new UpdateCountryCommand(data);
            var country = await _mediator.Send(command);

            if (country == null)
            {
                var notFound = req.CreateResponse(HttpStatusCode.NotFound);
                await notFound.WriteStringAsync("Country not found.");
                return notFound;
            }

            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(country);
            return response;
        }

        [Function("DeleteCountry")]
        public async Task<HttpResponseData> DeleteCountryAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "country/{id:int}")]
                HttpRequestData req,
            int id
        )
        {
            var result = await _mediator.Send(new DeleteCountryCommand(id));
            var response = req.CreateResponse(
                result ? HttpStatusCode.NoContent : HttpStatusCode.NotFound
            );
            if (!result)
                await response.WriteStringAsync("Country not found.");
            return response;
        }

        [Function("SearchCountries")]
        public async Task<IActionResult> SearchCountriesAsync(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "country/search")]
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

            var result = await _mediator.Send(new SearchCountryQuery(pagedQuery));
            return new OkObjectResult(result);
        }
    }
}

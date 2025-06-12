using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ai_addresses_api.Middlewares
{
    public class GlobalExceptionMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var req = await context.GetHttpRequestDataAsync();
                if (req != null)
                {
                    var res = req.CreateResponse(HttpStatusCode.InternalServerError);
                    var error = new
                    {
                        message = "An unexpected error occurred",
                        detail = ex.Message
                    };
                    await res.WriteStringAsync(JsonSerializer.Serialize(error));
                    context.GetInvocationResult().Value = res;
                }
                else
                {
                    // For non-HTTP triggers, just rethrow
                    throw;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;

namespace ai_addresses_api.Middlewares
{
    public class ApiKeyMiddleware : IFunctionsWorkerMiddleware
    {
        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            if (httpRequestData != null)
            {
                // Check for X_API_KEY header
                if (!httpRequestData.Headers.TryGetValues("X_API_KEY", out var apiKeyHeaders))
                {
                    throw new UnauthorizedAccessException("API key is missing.");
                }

                var apiKey = apiKeyHeaders.FirstOrDefault();
                if (string.IsNullOrEmpty(apiKey) || apiKey != SecretUtility.APIKey)
                {
                    throw new UnauthorizedAccessException("Invalid API key.");
                }
            }

            await next(context);
        }
    }
}

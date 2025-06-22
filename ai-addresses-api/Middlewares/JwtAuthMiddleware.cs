using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Middleware;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace ai_addresses_api.Middlewares
{
    public class JwtAuthMiddleware : IFunctionsWorkerMiddleware
    {
        private static readonly string GoogleIssuer = "https://accounts.google.com";
        private static readonly string GoogleJwksUri = "https://www.googleapis.com/oauth2/v3/certs";
        private static readonly string? GoogleClientId = SecretUtility.GOOGLE_CLIENT_ID;

        private static IEnumerable<SecurityKey>? _cachedKeys;
        private static DateTime _keysExpiry = DateTime.MinValue;

        public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next)
        {
            var httpRequestData = await context.GetHttpRequestDataAsync();
            if (httpRequestData != null)
            {
                if (!httpRequestData.Headers.TryGetValues("Authorization", out var authHeaders))
                    throw new UnauthorizedAccessException("Authorization header missing.");

                var bearer = authHeaders.FirstOrDefault();
                if (string.IsNullOrEmpty(bearer) || !bearer.StartsWith("Bearer "))
                    throw new UnauthorizedAccessException("Invalid Authorization header.");

                var token = bearer.Substring("Bearer ".Length);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = GoogleIssuer,
                    ValidateAudience = true,
                    ValidAudience = GoogleClientId,
                    ValidateLifetime = true,
                    RequireSignedTokens = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = await GetGoogleSigningKeysAsync(),
                };

                var handler = new JsonWebTokenHandler();
                var result = await handler.ValidateTokenAsync(token, validationParameters);

                if (!result.IsValid)
                {
                    throw new UnauthorizedAccessException(
                        $"Invalid JWT token: {result.Exception?.Message ?? result.Exception.Message}"
                    );
                }
            }

            await next(context);
        }

        private static async Task<IEnumerable<SecurityKey>> GetGoogleSigningKeysAsync()
        {
            if (_cachedKeys != null && DateTime.UtcNow < _keysExpiry)
                return _cachedKeys;

            using var httpClient = new HttpClient();
            var json = await httpClient.GetStringAsync(GoogleJwksUri);
            var keys = new JsonWebKeySet(json).GetSigningKeys();
            _cachedKeys = keys;
            _keysExpiry = DateTime.UtcNow.AddHours(1); // cache for 1 hour
            return keys;
        }
    }
}

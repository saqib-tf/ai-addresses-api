using System;
using System.Linq;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Net.Http.Headers;

namespace ai_addresses_api
{
    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary=---------------------------293582696224464
        public static string GetBoundary(HttpHeadersCollection headers, int lengthLimit)
        {
            var contentType = headers.GetValues("Content-Type").FirstOrDefault();
            if (contentType == null)
                throw new InvalidOperationException("Missing Content-Type header.");

            var elements = contentType.Split(';');
            var boundaryElement = elements.FirstOrDefault(e =>
                e.Trim().StartsWith("boundary=", StringComparison.OrdinalIgnoreCase)
            );
            if (boundaryElement == null)
                throw new InvalidOperationException("Missing boundary in Content-Type header.");

            var boundary = boundaryElement.Substring("boundary=".Length).Trim('"').Trim();
            if (string.IsNullOrWhiteSpace(boundary))
                throw new InvalidOperationException("Boundary value is empty.");

            if (boundary.Length > lengthLimit)
                throw new InvalidOperationException(
                    $"Boundary length limit {lengthLimit} exceeded."
                );

            return boundary;
        }
    }
}

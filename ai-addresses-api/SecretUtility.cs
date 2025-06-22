using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_addresses_api
{
    public class SecretUtility
    {
        public static string? AZURE_BLOB_CONNECTION_STRING =>
            Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING");

        public static string? SQL_SERVER_CONNECTION_STRING =>
            Environment.GetEnvironmentVariable("SQL_SERVER_CONNECTION_STRING");

        public static string? APIKey => Environment.GetEnvironmentVariable("X_API_KEY");

        public static string? GOOGLE_CLIENT_ID => Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
    }
}

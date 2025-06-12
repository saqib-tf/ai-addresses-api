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
    }
}

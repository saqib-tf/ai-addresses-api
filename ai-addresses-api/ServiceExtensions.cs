using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ai_addresses_data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ai_addresses_api
{
    public static class ServiceExtensions
    {
        public static void LoadEnvFileLocal(this IServiceCollection services)
        {
            DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        }

        public static void ConfigureDatabaseConnectionString(this IServiceCollection services)
        {
            // Database context
            services.AddDbContext<AddressBookDbContext>(options =>
                options.UseSqlServer(SecretUtility.SQL_SERVER_CONNECTION_STRING)
            );
        }
    }
}

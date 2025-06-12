using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ai_addresses_api
{
    public static class ServiceExtensions
    {
        public static void ConfigureEnvironmentVariables(this IServiceCollection services)
        {
            DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));
        }
    }
}

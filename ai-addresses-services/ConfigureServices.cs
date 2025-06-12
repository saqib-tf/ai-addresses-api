using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ai_addresses_services.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace ai_addresses_services
{
    public static class ConfigureServices
    {
        public static void ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly())
            );
            services.AddValidatorsFromAssemblyContaining<GenderValidator>();
            services.AddAutoMapper(typeof(MappingProfile));
        }
    }
}

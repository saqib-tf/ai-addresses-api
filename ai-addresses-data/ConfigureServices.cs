using ai_addresses_data.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ai_addresses_data
{
    public static class ConfigureServices
    {
        public static void ConfigureInfrastructureServices(this IServiceCollection services,
          IConfiguration config)
        {
            // Database context
            services.AddDbContext<AddressBookDbContext>(options =>
              options.UseSqlServer(config.GetConnectionString("DefaultConnection")));
            
            // Repository manager
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        }
    }
}

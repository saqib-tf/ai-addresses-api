using System.Reflection;
using ai_addresses_api;
using ai_addresses_api.Middlewares;
using ai_addresses_data;
using ai_addresses_data.Repository;
using ai_addresses_services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration(config =>
        config
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json")
            .AddEnvironmentVariables()
    )
    .ConfigureFunctionsWebApplication(builder =>
    {
        builder.UseMiddleware<GlobalExceptionMiddleware>();
        //builder.UseMiddleware<ApiKeyMiddleware>();
        builder.UseMiddleware<JwtAuthMiddleware>();
    })
    .ConfigureServices(
        (appBuilder, services) =>
        {
            var configuration = appBuilder.Configuration;

            //services.AddApplicationInsightsTelemetryWorkerService();
            //services.ConfigureFunctionsApplicationInsights();

            if (appBuilder.HostingEnvironment.IsDevelopment())
            {
                services.LoadEnvFileLocal();
            }

            services.ConfigureDatabaseConnectionString();
            services.ConfigureApplicationServices();
            services.ConfigureInfrastructureServices(configuration);
        }
    )
    .Build();

host.Run();

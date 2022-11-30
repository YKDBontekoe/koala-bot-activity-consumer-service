using Koala.ActivityConsumerService.Options;
using Koala.ActivityConsumerService.Repositories;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Koala.ActivityConsumerService.Services;
using Koala.ActivityConsumerService.Services.Interfaces;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Koala.ActivityConsumerService;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var host = Host
            .CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
                {
                    var env = context.HostingEnvironment;

                    builder
                        .SetBasePath(env.ContentRootPath)
                        .AddJsonFile("appsettings.json", true, true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true)
                        .AddEnvironmentVariables();
                }
            )
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureOptions(services, hostContext.Configuration);
                ConfigureServiceBus(services);

                services.AddTransient<IActivityRepository, ActivityRepository>();
                services.AddTransient<IMessageConsumerService, MessageConsumerService>();
                services.AddHostedService<ActivityConsumerWorker>();
            })
            .UseConsoleLifetime()
            .Build();

        await host.RunAsync();
    }
    
    // Configure options for the application to use in the services
    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.ServiceBus));
        services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.CosmosDb));
    }

    // Configure the Azure Service Bus client with the connection string
    private static void ConfigureServiceBus(IServiceCollection services)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(services.BuildServiceProvider().GetRequiredService<IOptions<ServiceBusOptions>>().Value.ConnectionString);
        });
    }
}
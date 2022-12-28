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
using Neo4j.Driver;
using Neo4jClient;
using Newtonsoft.Json.Serialization;

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
            .ConfigureServices(ConfigureDelegate)
            .UseConsoleLifetime()
            .Build();

        await host.RunAsync();
    }

    private static void ConfigureDelegate(HostBuilderContext hostContext, IServiceCollection services)
    {
        ConfigureOptions(services, hostContext.Configuration);
        ConfigureServiceBus(services);
        ConfigureNeo4JClient(services);

        services.AddTransient<IActivityCosmosRepository, ActivityCosmosRepository>();
        services.AddTransient<IActivityNeoRepository, ActivityNeoRepository>();
        services.AddTransient<IActivityService, ActivityService>();
        services.AddTransient<IMessageConsumerService, MessageConsumerService>();
        services.AddHostedService<ActivityConsumerWorker>();
    }

    // Configure options for the application to use in the services
    private static void ConfigureOptions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.ServiceBus));
        services.Configure<CosmosDbOptions>(configuration.GetSection(CosmosDbOptions.CosmosDb));
        services.Configure<Neo4JDbOptions>(configuration.GetSection(Neo4JDbOptions.Neo4jDb));
    }

    // Configure the Azure Service Bus client with the connection string
    private static void ConfigureServiceBus(IServiceCollection services)
    {
        services.AddAzureClients(builder =>
        {
            builder.AddServiceBusClient(services.BuildServiceProvider().GetRequiredService<IOptions<ServiceBusOptions>>().Value.ConnectionString);
        });
    }
    
    //Configure Neo4jClient with connection string and credentials
    private static void ConfigureNeo4JClient(IServiceCollection services)
    {
        IBoltGraphClient ImplementationInstance(IServiceProvider x)
        {
            // Get the necessary options from the service provider
            var options = services.BuildServiceProvider().GetRequiredService<IOptions<Neo4JDbOptions>>().Value;
            
            var driver = GraphDatabase.Driver(new Uri(options.Uri), AuthTokens.Basic(options.Username, options.Password), config => config.WithEncryptionLevel(EncryptionLevel.Encrypted));

            // Initialize the neo4JClient object using the options
            var neo4JClient = new BoltGraphClient(driver);
            neo4JClient.JsonContractResolver = new CamelCasePropertyNamesContractResolver();

            // Connect to the database asynchronously
            try
            {
                neo4JClient.ConnectAsync().Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            return neo4JClient;
        }

        services.AddSingleton(ImplementationInstance);
    }
}
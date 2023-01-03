using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Options;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Koala.ActivityConsumerService.Repositories;

public class ActivityCosmosRepository : IActivityCosmosRepository
{
    private readonly CosmosClient _database;
    private readonly CosmosDbOptions _options;

    public ActivityCosmosRepository(IOptions<CosmosDbOptions> cosmosDbOptions)
    {
        _options = cosmosDbOptions != null
            ? cosmosDbOptions.Value
            : throw new ArgumentNullException(nameof(cosmosDbOptions));
        _database = new CosmosClient(_options.ConnectionString);
    }

    public async Task AddActivityAsync(Activity activity)
    {
        var container = _database.GetContainer(_options.DatabaseName, _options.ContainerName);
        await container.UpsertItemAsync(activity);
    }
}
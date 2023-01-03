using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Koala.ActivityConsumerService.Services.Interfaces;

namespace Koala.ActivityConsumerService.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityCosmosRepository _activityCosmosRepository;
    private readonly IActivityNeoRepository _activityNeoRepository;

    public ActivityService(IActivityNeoRepository activityNeoRepository,
        IActivityCosmosRepository activityCosmosRepository)
    {
        _activityNeoRepository = activityNeoRepository;
        _activityCosmosRepository = activityCosmosRepository;
    }

    public async Task AddActivityAsync<T>(T activity) where T : Activity
    {
        // Add to Neo4j
        await _activityNeoRepository.GenerateRelationships(activity);

        // Add to CosmosDB
        await _activityCosmosRepository.AddActivityAsync(activity);
    }
}
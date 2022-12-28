using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Koala.ActivityConsumerService.Services.Interfaces;

namespace Koala.ActivityConsumerService.Services;

public class ActivityService : IActivityService
{
    private readonly IActivityNeoRepository _activityNeoRepository;
    private readonly IActivityCosmosRepository _activityCosmosRepository;

    public ActivityService(IActivityNeoRepository activityNeoRepository, IActivityCosmosRepository activityCosmosRepository)
    {
        _activityNeoRepository = activityNeoRepository;
        _activityCosmosRepository = activityCosmosRepository;
    }

    public async Task AddActivityAsync(Activity activity)
    {
        // Add to Neo4j
        await _activityNeoRepository.GenerateRelationships(activity);
        
        // Add to CosmosDB
        await _activityCosmosRepository.AddActivityAsync(activity);
    }
}
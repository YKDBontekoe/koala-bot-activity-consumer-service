using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Spotify;

public abstract class BaseSpotifyRelationshipCreationStrategy : BaseSpotifyCreationStrategy,
    IRelationshipCreationStrategy<SpotifyActivity>
{
    protected readonly IBoltGraphClient Client;

    protected BaseSpotifyRelationshipCreationStrategy(IBoltGraphClient client)
    {
        Client = client;
    }

    public abstract Task CreateRelationship(SpotifyActivity activity);
}
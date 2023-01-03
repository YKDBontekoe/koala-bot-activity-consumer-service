using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Spotify;

public abstract class BaseSpotifyNodeCreationStrategy : BaseSpotifyCreationStrategy, INodeCreationStrategy<SpotifyActivity>
{
    protected readonly IBoltGraphClient Client;

    protected BaseSpotifyNodeCreationStrategy(IBoltGraphClient client)
    {
        Client = client;
    }

    public abstract Task CreateNode(SpotifyActivity activity);
}
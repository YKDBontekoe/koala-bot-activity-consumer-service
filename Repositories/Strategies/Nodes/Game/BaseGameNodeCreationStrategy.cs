using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Game;

public abstract class BaseGameNodeCreationStrategy : BaseGameCreationStrategy, INodeCreationStrategy<GameActivity>
{
    protected readonly IBoltGraphClient Client;

    protected BaseGameNodeCreationStrategy(IBoltGraphClient client)
    {
        Client = client;
    }

    public abstract Task CreateNode(GameActivity activity);
}
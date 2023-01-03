using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Game;

public abstract class BaseGameRelationshipCreationStrategy : IRelationshipCreationStrategy<GameActivity>
{
    protected readonly IBoltGraphClient Client;

    protected BaseGameRelationshipCreationStrategy(IBoltGraphClient client)
    {
        Client = client;
    }

    public abstract Task CreateRelationship(GameActivity activity);

    public bool IsActivityValid(GameActivity activity)
    {
        return activity.Type.Equals(MessageTypes.Playing);
    }
}
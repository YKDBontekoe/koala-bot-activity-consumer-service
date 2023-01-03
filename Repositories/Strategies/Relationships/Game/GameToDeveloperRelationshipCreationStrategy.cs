using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Game;

public class GameToDeveloperRelationshipCreationStrategy : BaseGameRelationshipCreationStrategy
{
    public GameToDeveloperRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(GameActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        foreach (var developer in activity.GameInfo.Developers)
            await Client.Cypher
                .Match("(d:Developer)", "(g:Game)")
                .Where((DeveloperEntity d) => d.Name == developer)
                .AndWhere((GameEntity a) => a.Name == activity.Name)
                .Merge("(g)-[:IS_DEVELOPED_BY]->(d)")
                .ExecuteWithoutResultsAsync();
    }
}
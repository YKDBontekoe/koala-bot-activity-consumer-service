using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Game;

public class GameToFeatureTagRelationshipCreationStrategy : BaseGameRelationshipCreationStrategy
{
    public GameToFeatureTagRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(GameActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        foreach (var tag in activity.GameInfo.Tags)
            await Client.Cypher
                .Match("(f:Feature)", "(g:Game)")
                .Where((FeatureTagEntity f) => f.Name == tag)
                .AndWhere((GameEntity a) => a.Name == activity.Name)
                .Merge("(g)-[:HAS_TAG]->(f)")
                .ExecuteWithoutResultsAsync();
    }
}
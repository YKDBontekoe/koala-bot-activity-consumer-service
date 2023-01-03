using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Game;

public class GameToAllTimeReviewScoreRelationshipCreationStrategy : BaseGameRelationshipCreationStrategy
{
    public GameToAllTimeReviewScoreRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(GameActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        await Client.Cypher
            .Match("(rs:ReviewScore)", "(g:Game)")
            .Where((ReviewScoreEntity rs) => rs.Name == activity.GameInfo.AllTimeReviewScore)
            .AndWhere((GameEntity a) => a.Name == activity.Name)
            .Merge("(g)-[:HAS_ALL_TIME_REVIEW_SCORE]->(d)")
            .ExecuteWithoutResultsAsync();
    }
}
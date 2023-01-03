using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Game;

public class ReviewScoreCreationStrategy : BaseGameNodeCreationStrategy
{
    public ReviewScoreCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateNode(GameActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        await Client.Cypher
            .Merge("(d:ReviewScore {name: $name})")
            .OnCreate()
            .Set("a = $reviewScore")
            .WithParams(new
            {
                name = activity.GameInfo.RecentReviewScore,
                reviewScore = new ReviewScoreEntity
                {
                    Name = activity.GameInfo.RecentReviewScore
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}
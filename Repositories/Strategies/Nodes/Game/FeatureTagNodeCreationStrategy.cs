using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Game;

public class FeatureTagNodeCreationStrategy : BaseGameNodeCreationStrategy
{
    public FeatureTagNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateNode(GameActivity activity)
    {
        if (!IsActivityValid(activity)) return;


        foreach (var tag in activity.GameInfo.Tags)
            await Client.Cypher
                .Merge("(d:Feature {name: $name})")
                .OnCreate()
                .Set("a = $tag")
                .WithParams(new
                {
                    name = tag,
                    tag = new FeatureTagEntity
                    {
                        Name = tag
                    }
                })
                .ExecuteWithoutResultsAsync();
    }
}
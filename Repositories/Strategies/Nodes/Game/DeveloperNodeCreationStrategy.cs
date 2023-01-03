using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Game;

public class DeveloperNodeCreationStrategy : BaseGameNodeCreationStrategy
{
    public DeveloperNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateNode(GameActivity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }
        
        foreach (var developer in activity.GameInfo.Developers)
        {
            await Client.Cypher
                .Merge("(d:Developer {name: $name})")
                .OnCreate()
                .Set("a = $developer")
                .WithParams(new
                {
                    name = developer,
                    developer = new DeveloperEntity
                    {
                        Name = developer
                    }
                })
                .ExecuteWithoutResultsAsync();
        }
    }
}
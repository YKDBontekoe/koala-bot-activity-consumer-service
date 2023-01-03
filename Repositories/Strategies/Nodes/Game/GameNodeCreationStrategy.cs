using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes.Game;

public class GameNodeCreationStrategy : BaseGameNodeCreationStrategy
{
    public GameNodeCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateNode(GameActivity activity)
    {
        if (!IsActivityValid(activity)) return;

        await Client.Cypher
            .Merge("(a:Game {name: $name})")
            .OnCreate()
            .Set("a = $game")
            .WithParams(new
            {
                name = activity.Name,
                game = new GameEntity
                {
                    Name = activity.Name
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}
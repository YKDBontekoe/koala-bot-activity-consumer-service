using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class GameNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public GameNodeCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateNode(Activity activity)
    {
        if (!activity.Type.Equals(MessageTypes.Playing))
        {
            return;
        }
        
        var gameActivity = (GameActivity)activity;
        await _client.Cypher
            .Merge("(a:Game {id: $id})")
            .OnCreate()
            .Set("a = $game")
            .WithParams(new
            {
                id = gameActivity.GameInfo.ApplicationId.ToString(),
                game = new GameEntity
                {
                    Name = gameActivity.Name,
                    Id = gameActivity.GameInfo.ApplicationId.ToString()
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}
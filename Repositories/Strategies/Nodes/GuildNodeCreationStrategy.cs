using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class GuildNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public GuildNodeCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateNode(Activity activity)
    {
        var spotifyActivity = (SpotifyActivity)activity;
        if (spotifyActivity.User.Guilds is null)
        {
            return;
        }
        
        foreach (var guild in activity.User.Guilds)
        {
            await _client.Cypher
                .Merge("(g:Guild {name: $name})")
                .OnCreate()
                .Set("g = $guild")
                .WithParams(new
                {
                    name = guild.Name,
                    guild = new GuildEntity()
                    {
                        Name = guild.Name
                    }
                })
                .ExecuteWithoutResultsAsync();
        }
    }
}
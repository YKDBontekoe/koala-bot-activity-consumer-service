using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class UserToGuildRelationshipCreationStrategy : IRelationshipCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public UserToGuildRelationshipCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateRelationship(Activity activity)
    {
        var spotifyActivity = (SpotifyActivity)activity;
        if (spotifyActivity.User.Guilds is null)
        {
            return;
        }
        
        foreach (var guild in activity.User.Guilds)
        {
            await _client.Cypher
                .Match("(u:User)", "(g:Guild)")
                .Where((UserEntity u) => u.UserName == activity.User.Username)
                .AndWhere((GuildEntity g) => g.Name == guild.Name)
                .Merge("(u)-[:MEMBER_OF]->(g)")
                .ExecuteWithoutResultsAsync();
        }
    }
}
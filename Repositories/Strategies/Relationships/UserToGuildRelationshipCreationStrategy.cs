using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class UserToGuildRelationshipCreationStrategy : IRelationshipCreationStrategy<Activity>
{
    private readonly IBoltGraphClient _client;

    public UserToGuildRelationshipCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateRelationship(Activity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }
  
        foreach (var guild in activity.User.Guilds)
        {
            await _client.Cypher
                .Match("(u:User)", "(g:Guild)")
                .Where((UserEntity u) => u.Name == activity.User.Username)
                .AndWhere((GuildEntity g) => g.Name == guild.Name)
                .Merge("(u)-[:MEMBER_OF]->(g)")
                .ExecuteWithoutResultsAsync();
        }
    }

    public bool IsActivityValid(Activity activity)
    {
        return activity.User.Guilds is not null && activity.User.Guilds.Any();
    }
}
using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships;

public class UserToGameRelationshipCreationStrategy : IRelationshipCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public UserToGameRelationshipCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateRelationship(Activity activity)
    {
        if (!activity.Type.Equals(MessageTypes.Playing))
        {
            return;
        }
        
        var gameActivity = (GameActivity)activity;
        await _client.Cypher
            .Match("(u:User)", "(g:Game)")
            .Where((UserEntity u) => u.UserName == activity.User.Username)
            .AndWhere((GameEntity a) => a.Id == gameActivity.GameInfo.ApplicationId.ToString())
            .Merge("(u)-[:IS_PLAYING]->(g)")
            .ExecuteWithoutResultsAsync();
    }
}
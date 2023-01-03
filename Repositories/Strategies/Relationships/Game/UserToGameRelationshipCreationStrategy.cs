using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Relationships.Game;

public class UserToGameRelationshipCreationStrategy : BaseGameRelationshipCreationStrategy
{
    public UserToGameRelationshipCreationStrategy(IBoltGraphClient client) : base(client)
    {
    }

    public override async Task CreateRelationship(GameActivity activity)
    {
        if (!IsActivityValid(activity))
        {
            return;
        }
        
        await Client.Cypher
            .Match("(u:User)", "(g:Game)")
            .Where((UserEntity u) => u.Name == activity.User.Username)
            .AndWhere((GameEntity a) => a.Name == activity.Name)
            .Merge("(u)-[:IS_PLAYING]->(g)")
            .ExecuteWithoutResultsAsync();
    }
}
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class UserNodeCreationStrategy : INodeCreationStrategy
{
    private readonly IBoltGraphClient _client;

    public UserNodeCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateNode(Activity activity)
    {
        await _client.Cypher
            .Merge("(u:User {id: $id})")
            .OnCreate()
            .Set("u = $user")
            .WithParams(new
            {
                id = activity.User.Id.ToString(),
                user = new UserEntity()
                {
                    UserName = activity.User.Username,
                    Id = activity.User.Id.ToString(),
                }
            })
            .ExecuteWithoutResultsAsync();
    }
}
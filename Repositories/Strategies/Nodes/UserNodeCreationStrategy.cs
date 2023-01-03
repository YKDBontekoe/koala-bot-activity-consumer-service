using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Models.Entities;
using Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;
using Neo4jClient;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Nodes;

public class UserNodeCreationStrategy : INodeCreationStrategy<Activity>
{
    private readonly IBoltGraphClient _client;

    public UserNodeCreationStrategy(IBoltGraphClient client)
    {
        _client = client;
    }

    public async Task CreateNode(Activity activity)
    {
        await _client.Cypher
            .Merge("(u:User {name: $name})")
            .OnCreate()
            .Set("u = $user")
            .WithParams(new
            {
                name = activity.User.Username,
                user = new UserEntity
                {
                    Name = activity.User.Username
                }
            })
            .ExecuteWithoutResultsAsync();
    }

    public bool IsActivityValid(Activity activity)
    {
        throw new NotImplementedException();
    }
}
using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;

public interface INodeCreationStrategy
{
    Task CreateNode(Activity activity);
}
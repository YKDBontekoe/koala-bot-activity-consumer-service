using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;

public interface INodeCreationStrategy<in T> : INeoStrategy<T> where T : Activity
{
    Task CreateNode(T activity);
}
using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;

public interface IRelationshipCreationStrategy<in T> : INeoStrategy<T> where T : Activity
{
    Task CreateRelationship(T activity);
}
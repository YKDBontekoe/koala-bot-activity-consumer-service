using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Strategies.Interfaces;

public interface IRelationshipCreationStrategy
{
    Task CreateRelationship(Activity activity);
}
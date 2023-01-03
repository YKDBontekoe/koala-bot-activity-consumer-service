using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Interfaces;

public interface IActivityNeoRepository
{
    Task GenerateRelationships(Activity activity);
}
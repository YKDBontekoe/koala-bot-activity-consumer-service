using Koala.ActivityConsumerService.Models;

namespace Koala.ActivityConsumerService.Repositories.Interfaces;

public interface IActivityNeoRepository
{
    Task GenerateRelationships(Activity activity);
}
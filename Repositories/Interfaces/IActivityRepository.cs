using Koala.ActivityConsumerService.Models;

namespace Koala.ActivityConsumerService.Repositories.Interfaces;

public interface IActivityRepository
{
    Task AddActivityAsync(Activity message);
}
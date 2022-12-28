using Koala.ActivityConsumerService.Models;

namespace Koala.ActivityConsumerService.Services.Interfaces;

public interface IActivityService
{
    Task AddActivityAsync(Activity activity);
}
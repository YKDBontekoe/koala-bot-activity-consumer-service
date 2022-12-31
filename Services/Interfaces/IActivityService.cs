using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Services.Interfaces;

public interface IActivityService
{
    Task AddActivityAsync(Activity activity);
}
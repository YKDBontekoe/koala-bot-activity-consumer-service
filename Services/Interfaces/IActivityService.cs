using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Services.Interfaces;

public interface IActivityService
{
    Task AddActivityAsync<T>(T activity) where T : Activity;
}
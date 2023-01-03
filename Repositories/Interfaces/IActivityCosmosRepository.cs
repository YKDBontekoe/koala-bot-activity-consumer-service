using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Interfaces;

public interface IActivityCosmosRepository
{
    Task AddActivityAsync<T>(T message)  where T : Activity;
}
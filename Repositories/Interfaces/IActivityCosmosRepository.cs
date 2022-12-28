using Koala.ActivityConsumerService.Models;

namespace Koala.ActivityConsumerService.Repositories.Interfaces;

public interface IActivityCosmosRepository
{
    Task AddActivityAsync(Activity message);
}
using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Interfaces;

public interface IActivityCosmosRepository
{
    Task AddActivityAsync(Activity message);
}
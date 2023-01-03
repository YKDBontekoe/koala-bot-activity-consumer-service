using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Strategies;

public abstract class BaseGameCreationStrategy
{
    public bool IsActivityValid(GameActivity activity)
    {
        return activity.Type.Equals(MessageTypes.Playing);
    }
}
using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Repositories.Strategies;

public abstract class BaseSpotifyCreationStrategy
{
    public bool IsActivityValid(SpotifyActivity activity)
    {
        return activity.Type.Equals(MessageTypes.Listening);
    }
}
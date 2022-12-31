using Koala.ActivityConsumerService.Models.Activities;

namespace Koala.ActivityConsumerService.Services.Strategies.Interfaces;

public interface IActivityDeserializationStrategy
{
    Activity? Deserialize(string json);
}
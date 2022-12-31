using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Services.Strategies.Interfaces;
using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Services.Strategies;

public class GameActivityDeserializationStrategy : IActivityDeserializationStrategy
{
    public Activity? Deserialize(string json) => JsonConvert.DeserializeObject<GameActivity>(json);
}
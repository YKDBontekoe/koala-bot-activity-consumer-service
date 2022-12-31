using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Services.Strategies.Interfaces;
using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Services.Strategies;

public class StreamingActivityDeserializationStrategy : IActivityDeserializationStrategy
{
    public Activity? Deserialize(string json) => JsonConvert.DeserializeObject<StreamingActivity>(json);
}
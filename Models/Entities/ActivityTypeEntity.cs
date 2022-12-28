using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class ActivityTypeEntity
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }
}
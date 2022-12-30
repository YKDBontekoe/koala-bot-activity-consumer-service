using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class ActivityEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; } = string.Empty;
}
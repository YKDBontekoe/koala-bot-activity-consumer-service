using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class TrackEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
    
    [JsonProperty(PropertyName = "durationInSeconds")]
    public int? DurationInSeconds { get; set; }
}
using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class GameEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = string.Empty;
}
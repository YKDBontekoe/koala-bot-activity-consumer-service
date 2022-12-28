using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class NeoEntity
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }
    
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}
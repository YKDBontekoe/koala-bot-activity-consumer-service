using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class NeoEntity
{
    [JsonProperty(PropertyName = "name")] public required string Name { get; set; }
}
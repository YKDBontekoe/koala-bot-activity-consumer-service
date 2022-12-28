using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class ArtistEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}
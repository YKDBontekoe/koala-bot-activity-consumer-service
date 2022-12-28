using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class AlbumEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}
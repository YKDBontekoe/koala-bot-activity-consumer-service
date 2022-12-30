using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class GenreEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}
using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class GuildEntity
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }
}
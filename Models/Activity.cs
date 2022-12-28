using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Koala.ActivityConsumerService.Models;

public class Activity
{
    [JsonProperty(PropertyName ="id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; }
    public string Name { get; set; }
    public string Details { get; set; }
    public SpotifyInfo? SpotifyInfo { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    
    public User User { get; set; }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Koala.ActivityConsumerService.Models;

public class Activity
{
    [JsonProperty(PropertyName ="id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Type { get; set; } = "Activity";
    public string Name { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
    public SpotifyInfo? SpotifyInfo { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    
    public User User { get; set; }
}
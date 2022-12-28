using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Models.Entities;

public class UserEntity
{
    [JsonProperty(PropertyName = "userName")]
    public string UserName { get; set; }
}
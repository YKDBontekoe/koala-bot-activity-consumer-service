using Koala.ActivityPublisherService.Models;

namespace Koala.ActivityConsumerService.Models.Activities;

public class StreamingActivity : Activity
{
    public required StreamingInfo StreamingInfo { get; set; }
}
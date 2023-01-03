namespace Koala.ActivityConsumerService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";

    public string ConnectionString { get; set; }

    public string MusicQueueName { get; set; }
    public string ActivitiesQueueName { get; set; }
    public string GameQueueName { get; set; }
}
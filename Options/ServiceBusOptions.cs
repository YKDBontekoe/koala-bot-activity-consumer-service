namespace Koala.ActivityConsumerService.Options;

public class ServiceBusOptions
{
    public const string ServiceBus = "ServiceBus";
    
    public string ConnectionString { get; set; }
    
    public string QueueName { get; set; }
}
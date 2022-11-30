namespace Koala.ActivityConsumerService.Services.Interfaces;

public interface IMessageConsumerService
{
    Task RegisterOnMessageHandlerAndReceiveMessages();
    Task? CloseQueueAsync();
    Task? DisposeAsync();
}
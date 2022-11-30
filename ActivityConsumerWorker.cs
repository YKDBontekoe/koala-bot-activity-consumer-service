using Koala.ActivityConsumerService.Services.Interfaces;
using Microsoft.Extensions.Hosting;

namespace Koala.ActivityConsumerService;

public class ActivityConsumerWorker : IHostedService
{
    private readonly IMessageConsumerService _messageConsumerService;

    public ActivityConsumerWorker(IMessageConsumerService messageConsumerService)
    {
        _messageConsumerService = messageConsumerService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _messageConsumerService.RegisterOnMessageHandlerAndReceiveMessages();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _messageConsumerService.DisposeAsync()!;
        await _messageConsumerService.CloseQueueAsync()!;
    }
}
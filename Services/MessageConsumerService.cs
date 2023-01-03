using Azure.Messaging.ServiceBus;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Options;
using Koala.ActivityConsumerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Services;

public class MessageConsumerService : IMessageConsumerService
{
    private readonly IActivityService _activityService;
    private readonly ServiceBusClient _client;
    private readonly ServiceBusOptions _serviceBusOptions;
    private ServiceBusProcessor? _activityProcessor;
    private ServiceBusProcessor? _gameProcessor;
    private ServiceBusProcessor? _musicProcessor;

    public MessageConsumerService(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions,
        IActivityService activityService)
    {
        _client = serviceBusClient;
        _activityService = activityService;
        _serviceBusOptions = serviceBusOptions.Value;
    }

    public async Task RegisterOnMessageHandlerAndReceiveMessages()
    {
        var serviceBusOptions = new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15),
            PrefetchCount = 100
        };

        _musicProcessor = _client.CreateProcessor(_serviceBusOptions.MusicQueueName, serviceBusOptions);
        _gameProcessor = _client.CreateProcessor(_serviceBusOptions.GameQueueName, serviceBusOptions);
        _activityProcessor = _client.CreateProcessor(_serviceBusOptions.ActivitiesQueueName, serviceBusOptions);

        try
        {
            // add handler to process messages
            _musicProcessor.ProcessMessageAsync += MusicMessageHandler;
            _gameProcessor.ProcessMessageAsync += GameMessageHandler;
            _activityProcessor.ProcessMessageAsync += ActivitiesMessageHandler;

            // add handler to process any errors
            _musicProcessor.ProcessErrorAsync += ErrorHandler;
            _gameProcessor.ProcessErrorAsync += ErrorHandler;
            _activityProcessor.ProcessErrorAsync += ErrorHandler;

            // Start processing
            await _musicProcessor.StartProcessingAsync();
            await _gameProcessor.StartProcessingAsync();
            await _activityProcessor.StartProcessingAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CloseQueueAsync()
    {
        if (_gameProcessor != null) await _gameProcessor.CloseAsync();
        if (_musicProcessor != null) await _musicProcessor.CloseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_gameProcessor != null) await _gameProcessor.DisposeAsync();
        if (_musicProcessor != null) await _musicProcessor.DisposeAsync();
    }

    // handle received messages
    private async Task GameMessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var activity = JsonConvert.DeserializeObject<GameActivity>(body);
        ArgumentNullException.ThrowIfNull(activity);

        await _activityService.AddActivityAsync(activity);
    }


    // handle received messages
    private async Task MusicMessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var activity = JsonConvert.DeserializeObject<SpotifyActivity>(body);
        ArgumentNullException.ThrowIfNull(activity);

        await _activityService.AddActivityAsync(activity);
    }

    // handle received messages
    private async Task ActivitiesMessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var activity = JsonConvert.DeserializeObject<Activity>(body);
        ArgumentNullException.ThrowIfNull(activity);

        await _activityService.AddActivityAsync(activity);
    }

    // handle any errors when receiving messages
    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
using Azure.Messaging.ServiceBus;
using Koala.ActivityConsumerService.Constants;
using Koala.ActivityConsumerService.Models;
using Koala.ActivityConsumerService.Models.Activities;
using Koala.ActivityConsumerService.Options;
using Koala.ActivityConsumerService.Repositories.Interfaces;
using Koala.ActivityConsumerService.Services.Interfaces;
using Koala.ActivityConsumerService.Services.Strategies;
using Koala.ActivityConsumerService.Services.Strategies.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Koala.ActivityConsumerService.Services;

public class MessageConsumerService : IMessageConsumerService
{
    private readonly ServiceBusClient _client;
    private ServiceBusProcessor? _processor;
    private readonly IActivityService _activityService;
    private readonly ServiceBusOptions _serviceBusOptions;
    private readonly Dictionary<string, IActivityDeserializationStrategy> _deserializationStrategies;

    public MessageConsumerService(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions, IActivityService activityService)
    {
        _client = serviceBusClient;
        _activityService = activityService;
        _serviceBusOptions = serviceBusOptions.Value;
        
        _deserializationStrategies = new Dictionary<string, IActivityDeserializationStrategy>
        {
            { MessageTypes.Listening, new SpotifyActivityDeserializationStrategy() },
            { MessageTypes.Playing, new GameActivityDeserializationStrategy() },
            { MessageTypes.Streaming, new StreamingActivityDeserializationStrategy() }
        };
    }

    public async Task RegisterOnMessageHandlerAndReceiveMessages()
    {
        _processor = _client.CreateProcessor(_serviceBusOptions.QueueName, new ServiceBusProcessorOptions
        {
            AutoCompleteMessages = true,
            MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(15),
            PrefetchCount = 100,
        });
        
        try
        {
            // add handler to process messages
            _processor.ProcessMessageAsync += MessageHandler;

            // add handler to process any errors
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public async Task CloseQueueAsync()
    {
        if (_processor != null) await _processor.CloseAsync();
    }

    public async Task DisposeAsync()
    {
        if (_processor != null) await _processor.DisposeAsync();
    }

    // handle received messages
    private async Task MessageHandler(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();
        var activity = JsonConvert.DeserializeObject<Activity>(body);
        ArgumentNullException.ThrowIfNull(activity);
        
        if (_deserializationStrategies.TryGetValue(activity.Type, out var deserializationStrategy))
        {
            activity = deserializationStrategy.Deserialize(body);
            
            ArgumentNullException.ThrowIfNull(activity);
            await _activityService.AddActivityAsync(activity);
        }
    }

// handle any errors when receiving messages
    private static Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine(args.Exception.ToString());
        return Task.CompletedTask;
    }
}
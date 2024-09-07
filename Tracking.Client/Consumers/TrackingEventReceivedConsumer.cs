using MassTransit;
using Microsoft.Extensions.Logging;
using Tracking.Contracts.Events;

namespace Tracking.Client.Consumers;

public class TrackingEventReceivedConsumer : IConsumer<TrackingEventReceived>
{
    private readonly ILogger<TrackingEventReceivedConsumer> _logger;

    public TrackingEventReceivedConsumer(ILogger<TrackingEventReceivedConsumer> logger)
    {
        _logger = logger;
    }

    public Task Consume(ConsumeContext<TrackingEventReceived> context)
    {
        _logger.LogInformation("Received event: {AccountId} - {Data}", context.Message.AccountId, context.Message.Data);
        return Task.CompletedTask;
    }
}
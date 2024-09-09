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
        _logger.LogInformation("{@Event}", context.Message);
        return Task.CompletedTask;
    }
}
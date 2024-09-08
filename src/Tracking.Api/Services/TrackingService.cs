using MassTransit;
using Tracking.Contracts.Events;
using Tracking.Ef;
using Tracking.Ef.Entities;

namespace Tracking.Services;

internal interface ITrackingService
{
    Task TrackDataAsync(long accountId, string data, CancellationToken cancellationToken = default);
}

internal class TrackingService : ITrackingService
{
    private readonly TrackingDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public TrackingService(TrackingDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task TrackDataAsync(long accountId, string data, CancellationToken cancellationToken = default)
    {
        var trackingEvent = new TrackingEvent { AccountId = accountId, Data = data };
        
        await _context.TrackingEvents.AddAsync(trackingEvent, cancellationToken);
        await _publishEndpoint.Publish(new TrackingEventReceived(accountId, data), cancellationToken);
        
        await _context.SaveChangesAsync(cancellationToken);
    }
}
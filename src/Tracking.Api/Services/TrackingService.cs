using MassTransit;
using OneOf;
using Tracking.Contracts.Events;
using Tracking.Ef;
using Tracking.Ef.Entities;
using Tracking.Exceptions;
using Tracking.Models;

namespace Tracking.Services;

internal interface ITrackingService
{
    Task<OneOf<TrackingEventModel, DatabaseInsertException>> TrackDataAsync(TrackingEventModel trackingEvent, CancellationToken cancellationToken = default);
}

internal class TrackingService : ITrackingService
{
    private readonly TrackingDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<TrackingService> _logger;

    public TrackingService(TrackingDbContext context,
        IPublishEndpoint publishEndpoint,
        ILogger<TrackingService> logger)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task<OneOf<TrackingEventModel, DatabaseInsertException>> TrackDataAsync(TrackingEventModel trackingEvent,
        CancellationToken cancellationToken = default)
    {
        var entity = new TrackingEvent
        {
            Id = Guid.NewGuid(),
            AccountId = trackingEvent.AccountId,
            Data = trackingEvent.Data,
            Timestamp = DateTimeOffset.UtcNow
        };
        
        // TrackingEventReceived is saved to Outbox table in same transaction as TrackingEvent is inserted into its table
        await _context.TrackingEvents.AddAsync(entity, cancellationToken);
        await _publishEndpoint.Publish(new TrackingEventReceived(entity.Id, entity.AccountId, entity.Data, entity.Timestamp),
            x => x.SetRoutingKey($"account-id-{entity.AccountId}"),
            cancellationToken);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation("Saved event '{EventId}' (AccountId: '{AccountId}')", entity.Id, entity.AccountId);
            
            return new TrackingEventModel
            {
                Id = entity.Id,
                AccountId = entity.AccountId,
                Data = entity.Data,
                Timestamp = entity.Timestamp
            };
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception while saving event '{EventId}': '{Message}'", entity.Id, e.Message);
            
            return new DatabaseInsertException(e.Message, entity.Id.ToString(), e);
        }
    }
}
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

    public TrackingService(TrackingDbContext context)
    {
        _context = context;
    }

    public async Task TrackDataAsync(long accountId, string data, CancellationToken cancellationToken = default)
    {
        var eventEntity = new Event { AccountId = accountId, Data = data };
        
        await _context.Events.AddAsync(eventEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
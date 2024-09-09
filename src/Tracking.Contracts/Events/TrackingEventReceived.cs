namespace Tracking.Contracts.Events;

public record TrackingEventReceived(Guid Id, long AccountId, string Data, DateTimeOffset Timestamp);
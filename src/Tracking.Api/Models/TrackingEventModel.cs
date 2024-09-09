namespace Tracking.Models;

public record TrackingEventModel
{
    public Guid? Id { get; set; }
    public required long AccountId { get; set; }
    public required string Data { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
}
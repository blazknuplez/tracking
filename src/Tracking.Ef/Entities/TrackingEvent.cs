namespace Tracking.Ef.Entities;

public class TrackingEvent
{
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string Data { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
}
﻿namespace Tracking.Ef.Entities;

public record TrackingEvent
{
    public Guid Id { get; set; }
    public long AccountId { get; set; }
    public string Data { get; set; } = null!;
    public DateTimeOffset Timestamp { get; set; }
}
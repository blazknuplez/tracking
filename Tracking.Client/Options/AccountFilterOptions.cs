﻿namespace Tracking.Client.Options;

public class AccountFilterOptions
{
    public List<long> AccountIds { get; set; } = [];
    public bool IsFilteringEnabled => AccountIds.Any();
}
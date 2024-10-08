﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Tracking.Ef.Entities;

namespace Tracking.Ef.Configurations;

internal class TrackingEventConfiguration : IEntityTypeConfiguration<TrackingEvent>
{
    public void Configure(EntityTypeBuilder<TrackingEvent> builder)
    {
        builder.ToTable("TrackingEvents");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AccountId).IsRequired();
        builder.Property(x => x.Data).HasMaxLength(5000).IsRequired();
        builder.Property(x => x.Timestamp).IsRequired();

        builder.HasOne<Account>().WithMany().HasForeignKey(c => c.AccountId);
    }
}
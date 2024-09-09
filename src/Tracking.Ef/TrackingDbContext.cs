using MassTransit;
using Microsoft.EntityFrameworkCore;
using Tracking.Ef.Configurations;
using Tracking.Ef.DataSeed;
using Tracking.Ef.Entities;

namespace Tracking.Ef;

public class TrackingDbContext : DbContext
{
    public TrackingDbContext(DbContextOptions<TrackingDbContext> options)
        : base(options)
    {
    }
    
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<TrackingEvent> TrackingEvents => Set<TrackingEvent>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AccountConfiguration).Assembly);
        builder.Entity<Account>().HasData(AccountsDataSeed.Data);
        
        builder.AddInboxStateEntity();
        builder.AddOutboxMessageEntity();
        builder.AddOutboxStateEntity();
    }
}
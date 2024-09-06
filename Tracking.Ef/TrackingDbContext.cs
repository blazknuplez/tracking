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
    public DbSet<Event> Events => Set<Event>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AccountConfiguration).Assembly);
        builder.Entity<Account>().HasData(AccountsDataSeed.Data);
    }
    
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<Event>().Where(x => x.State == EntityState.Added))
        {
            entry.Entity.Timestamp = DateTimeOffset.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
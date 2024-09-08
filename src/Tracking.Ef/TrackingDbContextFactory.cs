using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Tracking.Ef;

internal class TrackingDbContextFactory : IDesignTimeDbContextFactory<TrackingDbContext>
{
    public TrackingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<TrackingDbContext>();
        optionsBuilder.UseSqlServer("Data Source=host.docker.internal,1433;Initial Catalog=Tracking;User ID=SA;Password=2Secure*Password2");

        return new TrackingDbContext(optionsBuilder.Options);
    }
    
}
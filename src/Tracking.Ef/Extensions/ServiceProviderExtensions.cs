using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Tracking.Ef.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task EnsureTrackingDatabaseCreated(this IServiceProvider serviceProvider)
    {
        var created = false;
        var scope = serviceProvider.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<TrackingDbContext>();

        while (!created)
        {
            try
            {
                await databaseContext.Database.MigrateAsync();
                created = true;
            }
            catch
            {
                await Task.Delay(1000);
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;

namespace Tracking.Ef.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task EnsureTrackingDatabaseCreated(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var databaseContext = scope.ServiceProvider.GetRequiredService<TrackingDbContext>();
        await databaseContext.Database.EnsureCreatedAsync();
    }
}
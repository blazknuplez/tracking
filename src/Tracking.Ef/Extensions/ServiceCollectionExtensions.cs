using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tracking.Ef.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTrackingDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddDbContext<TrackingDbContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("TrackingDatabase"));
        });
    }
}
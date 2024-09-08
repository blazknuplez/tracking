using MassTransit;
using Tracking.Ef;

namespace Tracking.Extensions;

internal static class PublisherExtensions
{
    public static IServiceCollection AddPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddMassTransit(x =>
        {
            x.AddEntityFrameworkOutbox<TrackingDbContext>(o =>
            {
                o.QueryDelay = TimeSpan.FromSeconds(1);
                o.UseSqlServer();
                o.UseBusOutbox();
            });

            x.UsingRabbitMq((context,cfg) =>
            {
                var options = configuration.GetSection("Publisher");
                var host = options.GetSection("Host").Value ?? throw new ArgumentException("Host");
                var virtualHost = options.GetSection("VirtualHost").Value ?? throw new ArgumentException("VirtualHost");
                var username = options.GetSection("Username").Value ?? throw new ArgumentException("Username");
                var password = options.GetSection("Password").Value ?? throw new ArgumentException("Password");
                
                cfg.Host(host, virtualHost, h => { h.Username(username); h.Password(password); });
                cfg.ConfigureEndpoints(context);
            });
        });
    }
}
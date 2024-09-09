using MassTransit;
using RabbitMQ.Client;
using Tracking.Contracts.Events;
using Tracking.Ef;

namespace Tracking.Extensions;

internal static class PublisherExtensions
{
    public static IServiceCollection AddPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .Configure<RabbitMqTransportOptions>(configuration.GetSection("Publisher"))
            .AddMassTransit(x =>
            {
                x.AddEntityFrameworkOutbox<TrackingDbContext>(o =>
                {
                    o.QueryDelay = TimeSpan.FromSeconds(1);
                    o.UseSqlServer();
                    o.UseBusOutbox();
                });

                x.UsingRabbitMq((context,cfg) =>
                {
                    cfg.Publish<TrackingEventReceived>(p =>
                    {
                        p.ExchangeType = ExchangeType.Topic;
                    });
                    
                    cfg.ConfigureEndpoints(context);
                });
            });
    }
}
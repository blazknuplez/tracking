

using MassTransit;
using Microsoft.Extensions.Hosting;
using Tracking.Client.Consumers;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TrackingEventReceivedConsumer>();
            x.UsingRabbitMq((context,cfg) =>
            {
                cfg.Host("localhost", "/", h => { h.Username("guest"); h.Password("guest"); });
                cfg.ConfigureEndpoints(context);
            });
        });
    });

await host.RunConsoleAsync();

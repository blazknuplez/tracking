using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Tracking.Client.Consumers;
using Tracking.Client.Options;
using Tracking.Contracts.Events;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureHostConfiguration(configuration =>
    {
        configuration.AddJsonFile("appsettings.json");
        configuration.AddJsonFile("appsettings.Development.json", true);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.Configure<AccountFilterOptions>(configuration.GetSection("AccountFilter"));
        services.Configure<RabbitMqTransportOptions>(configuration.GetSection("Subscriber"));
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TrackingEventReceivedConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.ReceiveEndpoint(e =>
                {
                    e.ConfigureConsumeTopology = false;
                    
                    var options = context.GetService<IOptions<AccountFilterOptions>>();
                    foreach (var accountId in options?.Value.AccountIds ?? [])
                    {
                        e.Bind<TrackingEventReceived>(b =>
                        {
                            b.RoutingKey = $"account-id-{accountId}";
                            b.ExchangeType = ExchangeType.Topic;
                            // b.SetBindingArgument(TrackingHeaders.AccountId, accountId);
                        }); 
                    }
                    
                    e.ConfigureConsumer<TrackingEventReceivedConsumer>(context);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
    });

await host.RunConsoleAsync();
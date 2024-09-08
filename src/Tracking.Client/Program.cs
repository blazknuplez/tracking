using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tracking.Client.Consumers;
using Tracking.Client.Filters;
using Tracking.Client.Options;

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
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<TrackingEventReceivedConsumer>();
            
            var options = configuration.GetSection("Subscriber");
            var host = options.GetSection("Host").Value ?? throw new ArgumentException("Host");
            var virtualHost = options.GetSection("VirtualHost").Value ?? throw new ArgumentException("VirtualHost");
            var username = options.GetSection("Username").Value ?? throw new ArgumentException("Username");
            var password = options.GetSection("Password").Value ?? throw new ArgumentException("Password");
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(host, virtualHost, h => { h.Username(username); h.Password(password); });
                cfg.ConfigureEndpoints(context);
                cfg.UseConsumeFilter<AccountFilter>(context);
            });
        });
    });

await host.RunConsoleAsync();
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Testcontainers.MsSql;
using Testcontainers.RabbitMq;
using Xunit;

namespace Tracking.Api.IntegrationTests;

public class TrackingWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _mssqlContainer = new MsSqlBuilder().WithImage("mcr.microsoft.com/mssql/server:2022-latest").Build();
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

    public async Task InitializeAsync()
    {
        await Task.WhenAll(_mssqlContainer.StartAsync(), _rabbitMqContainer.StartAsync());
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(_mssqlContainer.StopAsync(), _rabbitMqContainer.StopAsync());
        await base.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        base.ConfigureWebHost(builder);
        builder.UseTestServer();
        builder.ConfigureAppConfiguration(configuration =>
        {
            // use connection strings from newly running containers
            var testConfiguration = new Dictionary<string, string?>
            {
                ["ConnectionStrings:TrackingDatabase"] = _mssqlContainer.GetConnectionString(),
                ["Publisher:Host"] = _rabbitMqContainer.Hostname,
                ["Publisher:Username"] = RabbitMqBuilder.DefaultUsername,
                ["Publisher:Password"] = RabbitMqBuilder.DefaultPassword,
            };

            configuration.AddInMemoryCollection(testConfiguration);
        });
    }
}
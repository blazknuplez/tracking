using System.Net;
using System.Net.Http.Json;
using Tracking.Models;
using Xunit;

namespace Tracking.Api.IntegrationTests;

public class TrackingEventApiIntegrationTests : IClassFixture<TrackingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TrackingEventApiIntegrationTests(TrackingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PostTrackingEvent_ShouldReturnStatus202()
    {
        var accountId = 1;
        var data = "test";
        var response = await _client.PostAsync($"/tracking-events/{accountId}?data={data}", null);
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }
    
    [Fact]
    public async Task PostTrackingEvent_InactiveAccountId_ShouldReturnStatus400()
    {
        var accountId = 2;
        var data = "test";
        var response = await _client.PostAsync($"/tracking-events/{accountId}?data={data}", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task PostTrackingEvent_InvalidAccountId_ShouldReturnStatus400()
    {
        var accountId = 0;
        var data = "test";
        var response = await _client.PostAsync($"/tracking-events/{accountId}?data={data}", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task PostTrackingEvent_MissingDescription_ShouldReturnStatus400()
    {
        var accountId = 1;
        var data = "test";
        var response = await _client.PostAsync($"/tracking-events/{accountId}", null);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
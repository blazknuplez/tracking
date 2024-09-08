using System.Net;
using System.Net.Http.Json;
using Tracking.Requests;
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
        var eventData = new PostEventBody("Test");
        var response = await _client.PostAsJsonAsync($"/events/{accountId}", eventData);
        Assert.Equal(HttpStatusCode.Accepted, response.StatusCode);
    }
    
    [Fact]
    public async Task PostTrackingEvent_InactiveAccountId_ShouldReturnStatus400()
    {
        var accountId = 2;
        var eventData = new PostEventBody("Test");
        var response = await _client.PostAsJsonAsync($"/events/{accountId}", eventData);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task PostTrackingEvent_InvalidAccountId_ShouldReturnStatus400()
    {
        var accountId = 0;
        var eventData = new PostEventBody("Test");
        var response = await _client.PostAsJsonAsync($"/events/{accountId}", eventData);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task PostTrackingEvent_MissingDescription_ShouldReturnStatus400()
    {
        var accountId = 1;
        var eventData = new PostEventBody("");
        var response = await _client.PostAsJsonAsync($"/events/{accountId}", eventData);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
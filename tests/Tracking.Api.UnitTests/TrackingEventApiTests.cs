using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Tracking.Exceptions;
using Tracking.Models;
using Tracking.Services;
using Xunit;

namespace Tracking.Api.UnitTests;

public class TrackingEventApiTests
{
    private readonly Mock<IValidator<TrackingEventModel>> _validatorMock = new(MockBehavior.Strict);
    private readonly Mock<ITrackingService> _trackingServiceMock = new(MockBehavior.Strict);

    [Fact]
    public async Task CreateTrackingEvent_ValidationFailed_ShouldReturn400()
    {
        var accountId = 1;
        var data = "";

        _validatorMock.Setup(x => x.ValidateAsync(It.Is<TrackingEventModel>(t => t.AccountId == accountId && t.Data == data),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Data", "Must not be whitespace or empty")
            }));
        
        var result = await TrackingEventApi.CreateTrackingEvent(accountId, data, _validatorMock.Object, _trackingServiceMock.Object);
        
        Assert.IsType<ValidationProblem>(result);
        var validationProblem = result as ValidationProblem;
        Assert.NotNull(validationProblem);
        Assert.Equal(400, validationProblem.StatusCode);
    }
    
    [Fact]
    public async Task CreateTrackingEvent_TrackingFailed_ShouldReturn500()
    {
        var accountId = 1;
        var data = "Test";

        _validatorMock.Setup(x => x.ValidateAsync(It.Is<TrackingEventModel>(t => t.AccountId == accountId && t.Data == data),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _trackingServiceMock.Setup(x => x.TrackDataAsync(It.Is<TrackingEventModel>(t => t.AccountId == accountId && t.Data == data),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DatabaseInsertException("Something went wrong", Guid.NewGuid().ToString()));

        var result = await TrackingEventApi.CreateTrackingEvent(accountId, data, _validatorMock.Object, _trackingServiceMock.Object);
        
        Assert.IsType<ProblemHttpResult>(result);
        var problemResult = result as ProblemHttpResult;
        Assert.NotNull(problemResult);
        Assert.Equal(500, problemResult.StatusCode);
    }
    
    [Fact]
    public async Task CreateTrackingEvent_ShouldReturn202()
    {
        var accountId = 1;
        var data = "Test";

        _validatorMock.Setup(x => x.ValidateAsync(It.Is<TrackingEventModel>(t => t.AccountId == accountId && t.Data == data),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _trackingServiceMock.Setup(x => x.TrackDataAsync(It.Is<TrackingEventModel>(t => t.AccountId == accountId && t.Data == data),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TrackingEventModel { Id = Guid.NewGuid(), AccountId = accountId, Data = data, Timestamp = DateTimeOffset.UtcNow});
        
        var result = await TrackingEventApi.CreateTrackingEvent(accountId, data, _validatorMock.Object, _trackingServiceMock.Object);
        
        Assert.IsType<Accepted>(result);
        var acceptedResult = result as Accepted;
        Assert.NotNull(acceptedResult);
        Assert.Equal(202, acceptedResult.StatusCode);
    }
}
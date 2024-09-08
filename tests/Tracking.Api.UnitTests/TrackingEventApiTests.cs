using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using Tracking.Ef.Entities;
using Tracking.Exceptions;
using Tracking.Requests;
using Tracking.Services;
using Xunit;

namespace Tracking.Api.UnitTests;

public class TrackingEventApiTests
{
    private readonly Mock<IValidator<PostEventRequest>> _validatorMock = new(MockBehavior.Strict);
    private readonly Mock<ITrackingService> _trackingServiceMock = new(MockBehavior.Strict);

    [Fact]
    public async Task CreateTrackingEvent_ValidationFailed_ShouldReturn400()
    {
        var request = new PostEventRequest(1, new PostEventBody(""));

        _validatorMock.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult(new List<ValidationFailure>
            {
                new("Data", "Must not be whitespace or empty")
            }));
        
        var result = await TrackingEventApi.CreateTrackingEvent(request, _validatorMock.Object, _trackingServiceMock.Object);
        
        Assert.IsType<ValidationProblem>(result);
        var validationProblem = result as ValidationProblem;
        Assert.NotNull(validationProblem);
        Assert.Equal(400, validationProblem.StatusCode);
    }
    
    [Fact]
    public async Task CreateTrackingEvent_TrackingFailed_ShouldReturn500()
    {
        var request = new PostEventRequest(1, new PostEventBody("Test data"));

        _validatorMock.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _trackingServiceMock.Setup(x => x.TrackDataAsync(request.AccountId, request.Body.Data, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DatabaseInsertException());
        
        var result = await TrackingEventApi.CreateTrackingEvent(request, _validatorMock.Object, _trackingServiceMock.Object);
        
        Assert.IsType<ProblemHttpResult>(result);
        var problemResult = result as ProblemHttpResult;
        Assert.NotNull(problemResult);
        Assert.Equal(500, problemResult.StatusCode);
    }
    
    [Fact]
    public async Task CreateTrackingEvent_ShouldReturn202()
    {
        var request = new PostEventRequest(1, new PostEventBody("Test data"));

        _validatorMock.Setup(x => x.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _trackingServiceMock.Setup(x => x.TrackDataAsync(request.AccountId, request.Body.Data, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TrackingEvent { Id = 1, AccountId = request.AccountId, Data = "12345", Timestamp = DateTimeOffset.UtcNow});
        
        var result = await TrackingEventApi.CreateTrackingEvent(request, _validatorMock.Object, _trackingServiceMock.Object);
        
        Assert.IsType<Accepted>(result);
        var acceptedResult = result as Accepted;
        Assert.NotNull(acceptedResult);
        Assert.Equal(202, acceptedResult.StatusCode);
    }
}
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tracking.Exceptions;
using Tracking.Models;
using Tracking.Services;

namespace Tracking;

public static class TrackingEventApi
{
    public static RouteGroupBuilder MapTrackingApiEndpoints(this RouteGroupBuilder groups)
    {
        groups.MapPost("/{accountId}", CreateTrackingEvent)
            .Produces(202)
            .ProducesValidationProblem();
        
        return groups;
    }
    
    internal static async Task<IResult> CreateTrackingEvent([FromRoute] long accountId,
        [FromQuery] string data,
        [FromServices] IValidator<TrackingEventModel> validator,
        [FromServices] ITrackingService trackingService,
        CancellationToken cancellationToken = default)
    {
        var trackingEvent = new TrackingEventModel { AccountId = accountId, Data = data };
        
        var validationResult = await validator.ValidateAsync(trackingEvent, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await trackingService.TrackDataAsync(trackingEvent, cancellationToken);

        return result.Match(
            success => Results.Accepted(),
            exception => Results.Problem(exception.ToProblemDetails())
        );
    }
}
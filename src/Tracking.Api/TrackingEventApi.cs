using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tracking.Exceptions;
using Tracking.Requests;
using Tracking.Services;

namespace Tracking;

public static class TrackingEventApi
{
    public static RouteGroupBuilder MapTrackingApiEndpoints(this RouteGroupBuilder groups)
    {
        groups.MapPost("/{accountId}", CreateTrackingEvent)
            .Accepts<PostEventRequest>("application/json")
            .Produces(202)
            .ProducesValidationProblem();
        
        return groups;
    }
    
    internal static async Task<IResult> CreateTrackingEvent([AsParameters]PostEventRequest request,
        [FromServices] IValidator<PostEventRequest> validator,
        [FromServices] ITrackingService trackingService,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        var result = await trackingService.TrackDataAsync(request.AccountId, request.Body.Data, cancellationToken);

        return result.Match(
            success => Results.Accepted(),
            exception => Results.Problem(exception.ToProblemDetails())
        );
    }
}
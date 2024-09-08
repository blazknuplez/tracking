using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Tracking.Ef.Extensions;
using Tracking.Extensions;
using Tracking.Requests;
using Tracking.Services;
using Tracking.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddScoped<IValidator<PostEventRequest>, PostEventRequestValidator>()
    .AddScoped<ITrackingService, TrackingService>()
    .AddTrackingDbContext(builder.Configuration)
    .AddPublisher(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("api/events/{accountId}", async ([AsParameters]PostEventRequest request,
    [FromServices] IValidator<PostEventRequest> validator,
    [FromServices] ITrackingService trackingService,
    CancellationToken cancellationToken = default) =>
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        await trackingService.TrackDataAsync(request.AccountId, request.Body.Data, cancellationToken);

        return Results.Accepted();
    })
    .Produces(202)
    .ProducesValidationProblem()
    .WithName("Post Events")
    .WithOpenApi();

await app.Services.EnsureTrackingDatabaseCreated();
await app.RunAsync();
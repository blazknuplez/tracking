using FluentValidation;
using Tracking;
using Tracking.Ef.Extensions;
using Tracking.Extensions;
using Tracking.Models;
using Tracking.Services;
using Tracking.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddScoped<IValidator<TrackingEventModel>, TrackingEventValidator>()
    .AddScoped<ITrackingService, TrackingService>()
    .AddTrackingDbContext(builder.Configuration)
    .AddPublisher(builder.Configuration);

var app = builder.Build();

// this would be only available for development
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGroup("/tracking-events")
    .MapTrackingApiEndpoints()
    .WithTags("Tracking events")
    .WithOpenApi();

await app.Services.EnsureTrackingDatabaseCreated();
await app.RunAsync();

// needed for integration tests
public partial class Program { }
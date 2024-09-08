using FluentValidation;
using Tracking;
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


app.MapGroup("/events")
    .MapTrackingApiEndpoints()
    .WithTags("Tracking events")
    .WithOpenApi();

await app.Services.EnsureTrackingDatabaseCreated();
await app.RunAsync();

// for integration tests
public partial class Program { }
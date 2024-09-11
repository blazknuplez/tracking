using Microsoft.AspNetCore.Mvc;

namespace Tracking.Exceptions;

internal class DatabaseInsertException : Exception
{
    public string EventId { get; set; }
    
    public DatabaseInsertException(string? message, string eventId , Exception? innerException = null)
        : base(message, innerException)
    {
        EventId = eventId;
    }
}

internal static class DatabaseExceptionExtensions
{
    public static ProblemDetails ToProblemDetails(this DatabaseInsertException exception)
    {
        return new ProblemDetails
        {
            Title = "Server error",
            Detail = $"Exception occured while trying to save event '{exception.EventId}' to database", // Would probably change EventId with CorrelationId in real implementation
            Status = 500
        };
    }
}
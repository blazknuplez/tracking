using Microsoft.AspNetCore.Mvc;

namespace Tracking.Exceptions;

internal class DatabaseInsertException : Exception
{
    public DatabaseInsertException(string? message = null, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}

internal static class DatabaseExceptionExtensions
{
    public static ProblemDetails ToProblemDetails(this DatabaseInsertException _)
    {
        return new ProblemDetails
        {
            Title = "Server error",
            Detail = "Exception occured while trying to save event to database",
            Status = 500
        };
    }
}
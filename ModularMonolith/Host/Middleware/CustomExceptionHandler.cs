using System.Net;
using Domain.Exceptions;
using WebHost;

namespace Api.Middleware;

public sealed class CustomExceptionHandler : ExceptionHandler
{
    public override (HttpStatusCode, ApiError) HandleException(Exception exception)
    {
        return exception switch
        {
            EntityNotFoundException ex => Handle(ex),
            _ => base.HandleException(exception)
        };
    }

    private static (HttpStatusCode, ApiError) Handle(EntityNotFoundException ex)
    {
        return (HttpStatusCode.NotFound, new ApiError("The requested resource was not found.", ex.Message));
    }
}

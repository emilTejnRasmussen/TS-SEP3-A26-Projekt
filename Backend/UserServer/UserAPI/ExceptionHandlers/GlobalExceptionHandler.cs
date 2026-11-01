using Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Repository.Exceptions;
using Services.Exceptions;

namespace UserAPI.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            UserNotFoundException => StatusCodes.Status404NotFound,
            EmailAlreadyExistsException => StatusCodes.Status409Conflict,
            InvalidCredentialsException => StatusCodes.Status401Unauthorized,
            InvalidCurrentPasswordException => StatusCodes.Status400BadRequest,
            ForbiddenOperationException => StatusCodes.Status403Forbidden,
            _ => 0
        };

        if (statusCode == 0)
        {
            return false;
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                error = exception.Message
            },
            cancellationToken
        );

        return true;
    }
}
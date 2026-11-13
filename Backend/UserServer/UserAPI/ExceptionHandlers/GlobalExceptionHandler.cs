using Exceptions;
using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Repository.Exceptions;
using Services.Exceptions;

namespace UserAPI.ExceptionHandlers;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
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
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            InvalidCurrentPasswordException => StatusCodes.Status400BadRequest,
            ForbiddenOperationException => StatusCodes.Status403Forbidden,
            
            RpcException rpc => rpc.StatusCode switch
            {
                StatusCode.InvalidArgument => StatusCodes.Status400BadRequest,
                StatusCode.NotFound => StatusCodes.Status404NotFound,
                StatusCode.AlreadyExists => StatusCodes.Status409Conflict,
                StatusCode.FailedPrecondition => StatusCodes.Status409Conflict,
                StatusCode.PermissionDenied => StatusCodes.Status403Forbidden,
                StatusCode.Unauthenticated => StatusCodes.Status502BadGateway,
                StatusCode.Internal => StatusCodes.Status500InternalServerError,
                StatusCode.Unavailable => StatusCodes.Status503ServiceUnavailable,
                StatusCode.DeadlineExceeded => StatusCodes.Status504GatewayTimeout,
                _ => StatusCodes.Status502BadGateway
            },

            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Request {Method} {Path} failed with HTTP {StatusCode}",
                httpContext.Request.Method, httpContext.Request.Path, statusCode);
        }

        httpContext.Response.StatusCode = statusCode;

        var errorMessage = exception switch
        {
            RpcException rpc when rpc.StatusCode is
                    StatusCode.InvalidArgument or
                    StatusCode.NotFound or
                    StatusCode.AlreadyExists or
                    StatusCode.FailedPrecondition
                => string.IsNullOrWhiteSpace(rpc.Status.Detail)
                    ? "The event request could not be processed."
                    : rpc.Status.Detail,

            RpcException rpc when rpc.StatusCode == StatusCode.Internal
                => "An unexpected error occurred on the event server",
            RpcException => "An error occurred while communicating with the event server",

            UnauthorizedAccessException => "Invalid user identity. Please log in again.",
            UserNotFoundException or
                EmailAlreadyExistsException or
                InvalidCredentialsException or
                InvalidCurrentPasswordException or
                ForbiddenOperationException => exception.Message,

            _ => "An unexpected error occurred"
        };

        await httpContext.Response.WriteAsJsonAsync(
            new
            {
                error = errorMessage
            },
            cancellationToken
        );

        return true;
    }
}

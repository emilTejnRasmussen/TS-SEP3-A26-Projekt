using Exceptions;
using Grpc.Core;
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
            
            RpcException rpc => rpc.StatusCode switch
            {
                StatusCode.InvalidArgument => 400,
                StatusCode.NotFound => 404,
                StatusCode.AlreadyExists => 409,
                StatusCode.FailedPrecondition => 409,
                StatusCode.PermissionDenied => 403,
                StatusCode.Unauthenticated => 502,
                StatusCode.Unavailable => 503,
                StatusCode.DeadlineExceeded => 504,
                _ => 502
            },

            _ => 0
        };

        if (statusCode == 0)
        {
            return false;
        }

        httpContext.Response.StatusCode = statusCode;

        var errorMessage = exception switch
        {
            RpcException rpc when rpc.StatusCode is
                    StatusCode.InvalidArgument or
                    StatusCode.NotFound or
                    StatusCode.AlreadyExists or
                    StatusCode.FailedPrecondition
                => rpc.Status.Detail,

            RpcException => "An error occurred while communicating with the event server",

            _ => exception.Message
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
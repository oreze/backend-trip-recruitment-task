using System.Net;
using System.Text.Json;
using BackendTripRecruitmentTask.Application.Dtos.Common;
using BackendTripRecruitmentTask.Domain.Exceptions;
using Serilog;
using Serilog.Context;
using ILogger = Serilog.ILogger;

namespace BackendTripRecruitmentTask.API.Middlewares;

/// <summary>
///     Middleware for handling custom exceptions. Transform exceptions into status codes returned by API.
/// </summary>
public class CustomExceptionHandlerMiddleware(RequestDelegate next)
{
    private const string InternalServerErrorMessage =
        "Something went wrong on our end. Please try again later or contact support if the issue persists. (Error ID: {0})";

    private static readonly ILogger Logger = Log.ForContext<CustomExceptionHandlerMiddleware>();

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public async Task Invoke(HttpContext context)
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var errorId = Guid.NewGuid();

            using (LogContext.PushProperty("ErrorId", errorId))
            {
                Logger.Error(exception, "Error during request execution: {Context}", context.Request.Path.Value);
            }

            var response = context.Response;
            response.ContentType = "application/json";

            var (status, message) = GetResponse(exception, errorId);
            response.StatusCode = (int)status;
            await response.WriteAsync(message);
        }
    }

    private static (HttpStatusCode code, string message) GetResponse(Exception exception, Guid errorId)
    {
        HttpStatusCode code;
        string? message = default;

        switch (exception)
        {
            case InputException:
#pragma warning disable CS0164 // This label has not been referenced
                TripRegistrationLimitExceededException:
#pragma warning restore CS0164 // This label has not been referenced
                code = HttpStatusCode.BadRequest;
                break;
            case NotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                message = string.Format(InternalServerErrorMessage, errorId);
                break;
        }

        var errorResponse = new ErrorResponse(message ?? exception.Message);
        return (code, JsonSerializer.Serialize(errorResponse));
    }
}
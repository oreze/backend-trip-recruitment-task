using System.Net;
using System.Text.Json;
using BackendTripRecruitmentTask.Application.Dtos.Common;
using BackendTripRecruitmentTask.Domain.Exceptions;
using Serilog;
using Serilog.Context;

namespace BackendTripRecruitmentTask.API.Middlewares;

public class CustomExceptionHandlerMiddleware(RequestDelegate next)
{
    private const string InternalServerErrorMessage = 
        "Something went wrong on our end. Please try again later or contact support if the issue persists. (Error ID: {0})";
    
    private static readonly Serilog.ILogger Logger = Log.ForContext<CustomExceptionHandlerMiddleware>();

    public async Task Invoke(HttpContext context)
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
                code = HttpStatusCode.BadRequest;
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
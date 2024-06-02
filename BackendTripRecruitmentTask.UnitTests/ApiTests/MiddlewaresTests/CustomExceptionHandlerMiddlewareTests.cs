using System.Net;
using System.Text.Json;
using BackendTripRecruitmentTask.API.Middlewares;
using BackendTripRecruitmentTask.Application.Dtos.Common;
using BackendTripRecruitmentTask.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Moq;

namespace BackendTripRecruitmentTask.UnitTests.ApiTests.MiddlewaresTests;

public class CustomExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task Invoke_NoException_CallsNextDelegate()
    {
        var nextDelegateMock = new Mock<RequestDelegate>();
        var middleware = new CustomExceptionHandlerMiddleware(nextDelegateMock.Object);
        var context = new DefaultHttpContext();

        await middleware.Invoke(context);

        nextDelegateMock.Verify(next => next(context), Times.Once);
        Assert.Equal(200, context.Response.StatusCode);
    }

    [Fact]
    public async Task Invoke_InputException_ReturnsBadRequest()
    {
        var exception = new InputException("Test", "Test message");
        var nextDelegateMock = new Mock<RequestDelegate>();
        nextDelegateMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);
        var middleware = new CustomExceptionHandlerMiddleware(nextDelegateMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.BadRequest, context.Response.StatusCode);
        var json = await ReadResponseBody(context);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json);
        Assert.Equal(exception.Message, errorResponse!.Message);
    }


    [Fact]
    public async Task Invoke_NotFoundException_ReturnsNotFound()
    {
        var exception = new NotFoundException("Test message");
        var nextDelegateMock = new Mock<RequestDelegate>();
        nextDelegateMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);
        var middleware = new CustomExceptionHandlerMiddleware(nextDelegateMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.NotFound, context.Response.StatusCode);
        var json = await ReadResponseBody(context);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json);
        Assert.Equal(exception.Message, errorResponse!.Message);
    }

    [Fact]
    public async Task Invoke_UnauthorizedAccessException_ReturnsUnauthorized()
    {
        var exception = new UnauthorizedAccessException("Test message");
        var nextDelegateMock = new Mock<RequestDelegate>();
        nextDelegateMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);
        var middleware = new CustomExceptionHandlerMiddleware(nextDelegateMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.Unauthorized, context.Response.StatusCode);
        var json = await ReadResponseBody(context);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json);
        Assert.Equal(exception.Message, errorResponse!.Message);
    }

    [Fact]
    public async Task Invoke_OtherException_ReturnsInternalServerError()
    {
        var exception = new Exception("Test message");
        var nextDelegateMock = new Mock<RequestDelegate>();
        nextDelegateMock.Setup(next => next(It.IsAny<HttpContext>()))
            .ThrowsAsync(exception);
        var middleware = new CustomExceptionHandlerMiddleware(nextDelegateMock.Object);
        var context = new DefaultHttpContext();
        context.Response.Body = new MemoryStream();

        await middleware.Invoke(context);

        Assert.Equal((int)HttpStatusCode.InternalServerError, context.Response.StatusCode);
        var json = await ReadResponseBody(context);
        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(json);
        Assert.StartsWith(
            "Something went wrong on our end. Please try again later or contact support if the issue persists. (Error ID: ",
            errorResponse!.Message);
    }

    private async Task<string> ReadResponseBody(HttpContext context)
    {
        context.Response.Body.Seek(0, SeekOrigin.Begin);
        return await new StreamReader(context.Response.Body).ReadToEndAsync();
    }
}
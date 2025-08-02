using CarRental.Api.Filters;
using CarRental.Api.Models;
using CarRental.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;

namespace CarRental.UnitTests.Api.Filters;

public class GlobalExceptionFilterTests
{
    private readonly Mock<ILogger<GlobalExceptionFilter>> _mockLogger;
    private readonly GlobalExceptionFilter _filter;

    public GlobalExceptionFilterTests()
    {
        _mockLogger = new Mock<ILogger<GlobalExceptionFilter>>();
        _filter = new GlobalExceptionFilter(_mockLogger.Object);
    }

    private ExceptionContext CreateExceptionContext(Exception exception)
    {
        var httpContext = new DefaultHttpContext();
        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor()
        );

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    [Fact]
    public void OnException_WhenDomainException_ReturnsBadRequest()
    {
        var exception = new DomainException("Domain exception message.");
        var context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = Assert.IsType<JsonResult>(context.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.True(context.ExceptionHandled);

        var errorResponse = Assert.IsType<ErrorResponse>(result.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
        Assert.Equal(exception.Message, errorResponse.Message);
    }

    [Fact]
    public void OnException_WhenArgumentException_ReturnsBadRequest()
    {
        var exception = new ArgumentException("Argument exception message.");
        var context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = Assert.IsType<JsonResult>(context.Result);
        Assert.Equal((int)HttpStatusCode.BadRequest, result.StatusCode);
        Assert.True(context.ExceptionHandled);

        var errorResponse = Assert.IsType<ErrorResponse>(result.Value);
        Assert.Equal((int)HttpStatusCode.BadRequest, errorResponse.StatusCode);
        Assert.Equal(exception.Message, errorResponse.Message);
    }

    [Fact]
    public void OnException_WhenApplicationException_ReturnsNotFound()
    {
        var exception = new ApplicationException("Application exception message.");
        var context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = Assert.IsType<JsonResult>(context.Result);
        Assert.Equal((int)HttpStatusCode.NotFound, result.StatusCode);
        Assert.True(context.ExceptionHandled);

        var errorResponse = Assert.IsType<ErrorResponse>(result.Value);
        Assert.Equal((int)HttpStatusCode.NotFound, errorResponse.StatusCode);
        Assert.Equal(exception.Message, errorResponse.Message);
    }

    [Fact]
    public void OnException_WhenUnhandledException_ReturnsInternalServerErrorAndLogsError()
    {
        var exception = new Exception("Unhandled exception.");
        var context = CreateExceptionContext(exception);

        _filter.OnException(context);

        var result = Assert.IsType<JsonResult>(context.Result);
        Assert.Equal((int)HttpStatusCode.InternalServerError, result.StatusCode);
        Assert.True(context.ExceptionHandled);

        var errorResponse = Assert.IsType<ErrorResponse>(result.Value);
        Assert.Equal((int)HttpStatusCode.InternalServerError, errorResponse.StatusCode);
        Assert.Equal("An unexpected error occurred.", errorResponse.Message);

        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
            Times.Once);
    }
}
using CarRental.Api.Models;
using CarRental.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace CarRental.Api.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        HttpStatusCode status = HttpStatusCode.InternalServerError;
        string message = "An unexpected error occurred.";
        var exceptionType = context.Exception.GetType();

        if (exceptionType == typeof(ArgumentException) || exceptionType == typeof(InvalidOperationException) || exceptionType == typeof(DomainException))
        {
            message = context.Exception.Message;
            status = HttpStatusCode.BadRequest;
        }
        else if (exceptionType == typeof(ApplicationException))
        {
            message = context.Exception.Message;
            status = HttpStatusCode.NotFound;
        }
        else
        {
            _logger.LogError(context.Exception, "Unhandled exception occurred.");
        }

        context.Result = new JsonResult(new ErrorResponse((int)status, message))
        {
            StatusCode = (int)status
        };
        context.ExceptionHandled = true;
    }
}
using AndOS.Application.Exceptions;
using AndOS.Domain.Exceptions;
using AndOS.Infrastructure.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AndOS.API;

public class ExceptionHandler : IExceptionHandler
{
    private ProblemDetails handleHttpRequestException(HttpRequestException exception)
    {
        var problemDetails = new ProblemDetails()
        {
            Status = (int)exception.StatusCode!,
            Title = "Http exception",
            Detail = exception.Message,
            Type = nameof(HttpRequestException)
        };

        return problemDetails;
    }

    private ProblemDetails handleValidationException(ValidationException exception)
    {
        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation exception",
            Detail = string.Join("\n", exception.Errors.Select((x) => x.ErrorMessage)),
            Type = nameof(ValidationException)
        };
        return problemDetails;
    }

    private ProblemDetails handleCommonException(Exception exception)
    {
        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Generic exception",
            Detail = exception.Message,
            Type = exception.GetType().Name
        };
        return problemDetails;
    }

    private ProblemDetails handlerLayerException(Exception exception)
    {
        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Layer exception",
            Detail = exception.Message,
            Type = exception.GetType().Name
        };

        return problemDetails;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ValidationException => handleValidationException((ValidationException)exception),
            HttpRequestException => handleHttpRequestException((HttpRequestException)exception),
            DomainLayerException or ApplicationLayerException or InfrastructureLayerException => handlerLayerException(exception),
            _ => handleCommonException(exception)
        };

        problemDetails.Instance = httpContext.Request.Path;
        httpContext.Response.StatusCode = problemDetails?.Status is int statusCode ? statusCode : StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}

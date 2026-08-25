using System.Net;
using Domain.Common;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebApi.Middlewares;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> _logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        var response = new Result();
        if (exception is FluentValidation.ValidationException fluentValidationException)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            List<string> validationErrors = new List<string>();
            foreach (var error in fluentValidationException.Errors)
            {
                validationErrors.Add(error.ErrorMessage);
            }
            response = Result.Failure(validationErrors, "Validation failed");

        }
        else if (exception is InvalidOperationException || exception is DomainException)
        {
            httpContext.Response.StatusCode = 400;
            response = Result.Failure(exception.Message);
        }
        else if (exception is ArgumentException)
        {
            httpContext.Response.StatusCode = 400;
            response = Result.Failure(exception.Message);
        }
        else
        {
            response = Result.Failure(exception.Message);
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        }



        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

   
}
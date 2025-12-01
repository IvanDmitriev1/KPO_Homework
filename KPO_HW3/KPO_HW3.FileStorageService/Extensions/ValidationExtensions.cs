using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace KPO_HW3.FileStorageService.Extensions;

internal static class ValidationExtensions
{
    public static ProblemDetails ToProblemDetails(this ValidationResult result, HttpContext httpContext)
    {
        var problem = new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = "One or more validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Instance = httpContext.Request.Path
        };

        var errors = result.Errors
            .GroupBy(static e => e.PropertyName)
            .ToDictionary(
                static g => g.Key,
                static g => g.Select(e => e.ErrorMessage).ToList()
            );

        problem.Extensions["errors"] = errors;

        return problem;
    }

}
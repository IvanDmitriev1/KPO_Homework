using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

namespace KPO_HW3.FileAnalysisService.Extensions;

internal static class ValidationExtensions
{
    public static ProblemDetails ToProblemDetails(this ValidationResult result, HttpContext httpContext)
    {
        var problem = new ProblemDetails
        {
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

    public static ProblemDetails ToProblemDetails(this ValidationException ex, HttpContext httpContext)
    {
        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation failed.",
            Detail = "See errors for details.",
            Instance = httpContext.Request.Path,
            Extensions =
            {
                ["errors"] = ex.Errors.Select(static e => new
                {
                    e.PropertyName,
                    e.ErrorMessage
                })
            }
        };

        return problem;
    }

}
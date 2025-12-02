using DotNext;
using FluentValidation;
using KPO_HW3.FileAnalysisService.Domain.Exceptions;
using System.Runtime.ExceptionServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KPO_HW3.FileAnalysisService.Extensions;

public static class ResultExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, HttpContext httpContext)
    {
        if (result.IsSuccessful)
            throw new InvalidOperationException("Result has a value");

        switch (result.Error)
        {
            case WorkNotFoundException:
                return TypedResults.NotFound();

            case ValidationException validationException:
                var validationProblem = validationException.ToProblemDetails(httpContext);
                return TypedResults.BadRequest(validationProblem);
        }

        ExceptionDispatchInfo.Throw(result.Error);
        throw new InvalidOperationException();
    }
}
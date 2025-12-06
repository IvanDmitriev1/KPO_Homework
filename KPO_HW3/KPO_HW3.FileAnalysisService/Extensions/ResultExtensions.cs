using DotNext;
using KPO_HW3.FileAnalysisService.Domain.Exceptions;
using System.Runtime.ExceptionServices;

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
        }

        ExceptionDispatchInfo.Throw(result.Error);
        throw new InvalidOperationException();
    }
}
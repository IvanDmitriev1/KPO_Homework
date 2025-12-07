using DotNext;
using KPO_HW3.Api.Infrastructure.Exceptions;
using System.Runtime.ExceptionServices;

namespace KPO_HW3.Api.Infrastructure.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccessful)
            throw new InvalidOperationException("Result has a value");

        switch (result.Error)
        {
            case WorkNotFoundException:
                return TypedResults.NotFound();
            case WorkAlreadyExistsException:
                return TypedResults.Conflict();
        }

        ExceptionDispatchInfo.Throw(result.Error);
        throw new InvalidOperationException();
    }
}
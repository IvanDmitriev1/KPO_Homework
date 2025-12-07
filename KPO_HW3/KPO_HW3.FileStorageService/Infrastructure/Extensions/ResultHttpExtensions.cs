using System.Runtime.ExceptionServices;
using DotNext;
using KPO_HW3.FileStorageService.Infrastructure.Exceptions;

namespace KPO_HW3.FileStorageService.Infrastructure.Extensions;

public static class ResultHttpExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result)
    {
        if (result.IsSuccessful)
            throw new InvalidOperationException("Result has a value");

        switch (result.Error)
        {
            case FileNotFoundException:
                return TypedResults.NotFound();
            case WorkAlreadyExistsException:
                return TypedResults.Conflict();
        }

        ExceptionDispatchInfo.Throw(result.Error);
        throw new InvalidOperationException();
    }
}
using DotNext;

namespace KPO_HW3.FileAnalysisService.Infrastructure.Extensions;

public static class ResultExtensions
{
    public static async Task<Result<TResult>> ContinueAsync<TSource, TResult>(
        this Task<Result<TSource>> resultTask,
        Func<TSource, Task<Result<TResult>>> next)
    {
        var result = await resultTask.ConfigureAwait(false);

        if (result.TryGet(out var value))
        {
            return await next.Invoke(value).ConfigureAwait(false);
        }

        return Result.FromException<TResult>(result.Error!);
    }
}
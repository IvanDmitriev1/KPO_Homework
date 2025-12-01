using Microsoft.AspNetCore.Mvc;

namespace KPO_HW3.FileStorageService.Extensions;

public static class HttpContextExtensions
{
    public static ProblemDetails CreateProblem(this HttpContext httpContext, int status, string title)
        => new()
        {
            Status = status,
            Title = title,
            Instance = httpContext.Request.Path
        };
}
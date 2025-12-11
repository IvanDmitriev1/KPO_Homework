using Microsoft.AspNetCore.Mvc;

namespace KPO_HW3.Api.Infrastructure.Exceptions;

public sealed class WorkAlreadyExistsException(string message, ProblemDetails? problem = null) : Exception(message)
{
    public ProblemDetails? Problem { get; } = problem;
}
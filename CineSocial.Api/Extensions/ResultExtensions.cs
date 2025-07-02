using CineSocial.Core.Shared;
using Microsoft.AspNetCore.Mvc;

namespace CineSocial.Api.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkResult();
        }

        return result.Error switch
        {
            var error when error.Contains("NOT_FOUND") => new NotFoundObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("CONFLICT") => new ConflictObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("VALIDATION") => new BadRequestObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("AUTH") => new UnauthorizedObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("AUTHZ") => new ObjectResult(new { error = result.Error, errors = result.Errors }) { StatusCode = 403 },
            _ => new ObjectResult(new { error = result.Error, errors = result.Errors }) { StatusCode = 500 }
        };
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(result.Data);
        }

        return result.Error switch
        {
            var error when error.Contains("NOT_FOUND") => new NotFoundObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("CONFLICT") => new ConflictObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("VALIDATION") => new BadRequestObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("AUTH") => new UnauthorizedObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("AUTHZ") => new ObjectResult(new { error = result.Error, errors = result.Errors }) { StatusCode = 403 },
            _ => new ObjectResult(new { error = result.Error, errors = result.Errors }) { StatusCode = 500 }
        };
    }

    public static IActionResult ToActionResult<T>(this PagedResult<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(new
            {
                data = result.Data,
                pagination = new
                {
                    pageNumber = result.PageNumber,
                    pageSize = result.PageSize,
                    totalCount = result.TotalCount,
                    totalPages = result.TotalPages,
                    hasPreviousPage = result.HasPreviousPage,
                    hasNextPage = result.HasNextPage
                }
            });
        }

        return result.Error switch
        {
            var error when error.Contains("NOT_FOUND") => new NotFoundObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("CONFLICT") => new ConflictObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("VALIDATION") => new BadRequestObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("AUTH") => new UnauthorizedObjectResult(new { error = result.Error, errors = result.Errors }),
            var error when error.Contains("AUTHZ") => new ObjectResult(new { error = result.Error, errors = result.Errors }) { StatusCode = 403 },
            _ => new ObjectResult(new { error = result.Error, errors = result.Errors }) { StatusCode = 500 }
        };
    }
}
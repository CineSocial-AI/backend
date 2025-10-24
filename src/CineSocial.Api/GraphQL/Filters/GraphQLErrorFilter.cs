using CineSocial.Application.Common.Exceptions;
using HotChocolate;

namespace CineSocial.Api.GraphQL.Filters;

/// <summary>
/// GraphQL error filter for formatting exceptions
/// </summary>
public class GraphQLErrorFilter : IErrorFilter
{
    private readonly IWebHostEnvironment _environment;

    public GraphQLErrorFilter(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public IError OnError(IError error)
    {
        if (error.Exception is null)
        {
            return error;
        }

        var builder = ErrorBuilder.FromError(error);

        switch (error.Exception)
        {
            case ValidationException validationException:
                builder
                    .SetMessage(validationException.Message)
                    .SetCode(validationException.ErrorCode)
                    .SetExtension("statusCode", validationException.StatusCode)
                    .SetExtension("validationErrors", validationException.Errors);
                break;

            case BaseException baseException:
                builder
                    .SetMessage(baseException.Message)
                    .SetCode(baseException.ErrorCode)
                    .SetExtension("statusCode", baseException.StatusCode);
                break;

            default:
                builder
                    .SetMessage(_environment.IsDevelopment()
                        ? error.Exception.Message
                        : "An unexpected error occurred.")
                    .SetCode("SERVER_001")
                    .SetExtension("statusCode", 500);
                break;
        }

        // Include stack trace only in development
        if (_environment.IsDevelopment() && error.Exception != null)
        {
            builder.SetExtension("stackTrace", error.Exception.StackTrace);
        }

        return builder.Build();
    }
}

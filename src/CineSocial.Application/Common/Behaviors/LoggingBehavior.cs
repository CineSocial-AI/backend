using System.Diagnostics;
using CineSocial.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that logs all requests and responses
/// Automatically includes TraceId from OpenTelemetry
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;

    public LoggingBehavior(
        ILogger<LoggingBehavior<TRequest, TResponse>> logger,
        ICurrentUserService currentUserService)
    {
        _logger = logger;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUserService.UserId ?? 0;
        var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace";

        // Log request start
        _logger.LogInformation(
            "Executing {RequestName} for User {UserId} | TraceId: {TraceId}",
            requestName, userId, traceId);

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();

            stopwatch.Stop();

            // Log successful completion
            _logger.LogInformation(
                "Completed {RequestName} for User {UserId} in {ElapsedMs}ms | TraceId: {TraceId}",
                requestName, userId, stopwatch.ElapsedMilliseconds, traceId);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            // Log failure
            _logger.LogError(ex,
                "Failed {RequestName} for User {UserId} after {ElapsedMs}ms | TraceId: {TraceId} | Error: {ErrorMessage}",
                requestName, userId, stopwatch.ElapsedMilliseconds, traceId, ex.Message);

            throw;
        }
    }
}

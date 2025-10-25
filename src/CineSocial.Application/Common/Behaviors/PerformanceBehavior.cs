using System.Diagnostics;
using CineSocial.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CineSocial.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that monitors performance and logs slow requests
/// Default threshold: 500ms
/// </summary>
public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<PerformanceBehavior<TRequest, TResponse>> _logger;
    private readonly ICurrentUserService _currentUserService;
    private const int SlowRequestThresholdMs = 500;

    public PerformanceBehavior(
        ILogger<PerformanceBehavior<TRequest, TResponse>> logger,
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
        var stopwatch = Stopwatch.StartNew();
        var response = await next();
        stopwatch.Stop();

        var elapsedMs = stopwatch.ElapsedMilliseconds;

        if (elapsedMs > SlowRequestThresholdMs)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _currentUserService.UserId ?? 0;
            var traceId = Activity.Current?.TraceId.ToString() ?? "no-trace";

            _logger.LogWarning(
                "Long running request detected: {RequestName} took {ElapsedMs}ms | User: {UserId} | TraceId: {TraceId}",
                requestName, elapsedMs, userId, traceId);
        }

        return response;
    }
}

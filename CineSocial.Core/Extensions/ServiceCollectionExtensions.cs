using System.Reflection;
using CineSocial.Core.Localization;
using CineSocial.Core.Logging;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CineSocial.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        services.AddValidatorsFromAssembly(assembly);

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddScoped<ILocalizationService, LocalizationService>();
        
        // Add logging services
        services.AddScoped<IDatabaseLogger, DatabaseLogger>();
        services.AddScoped<IAuthenticationLogger, AuthenticationLogger>();

        return services;
    }
}

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                var errors = failures.Select(x => x.ErrorMessage).ToList();
                
                if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(CineSocial.Core.Shared.Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var failureMethod = typeof(CineSocial.Core.Shared.Result).GetMethod("Failure", new[] { typeof(List<string>) })
                        ?.MakeGenericMethod(resultType);
                    return (TResponse)failureMethod?.Invoke(null, new object[] { errors })!;
                }
                else if (typeof(TResponse) == typeof(CineSocial.Core.Shared.Result))
                {
                    var failureMethod = typeof(CineSocial.Core.Shared.Result).GetMethod("Failure", new[] { typeof(List<string>) });
                    return (TResponse)failureMethod?.Invoke(null, new object[] { errors })!;
                }
                
                throw new ValidationException(failures);
            }
        }

        return await next();
    }
}
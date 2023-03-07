namespace Platipus.Wallet.Api.StartupSettings.ServicesRegistrations;

using Microsoft.Extensions.DependencyInjection;
using Application.Behaviors;

public static class BehaviorExtensions
{
    public static IServiceCollection AddAllBehaviors(this IServiceCollection services) => services;
            // .AddLoggingBehavior()
            // .AddLocalizationBehavior()
            // .AddValidationBehavior()
            // .AddExceptionBehavior();

    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
        => services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

    public static IServiceCollection AddLocalizationBehavior(this IServiceCollection services)
        => services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LocalizationBehavior<,>));

    public static IServiceCollection AddValidationBehavior(this IServiceCollection services)
        => services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    public static IServiceCollection AddExceptionBehavior(this IServiceCollection services)
        => services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ExceptionBehavior<,>));
}
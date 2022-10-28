namespace Platipus.Wallet.Api.StartupSettings;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

public static class StartupExtensions
{
    public static IMvcBuilder AddJsonOptions(
        this IMvcBuilder builder,
        string settingsName,
        Action<JsonOptions> configure)
    {
        builder.Services.Configure(settingsName, configure);
        builder.Services.AddSingleton<IConfigureOptions<MvcOptions>>(sp =>
        {
            var options = sp.GetRequiredService<IOptionsMonitor<JsonOptions>>();
            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            return new ConfigureMvcJsonOptions(settingsName, options, loggerFactory);
        });
        return builder;
    }
    
    public static string? GetJsonSettingsName(this HttpContext context)
    {
        return context.GetEndpoint()
            ?.Metadata
            .GetMetadata<JsonSettingsNameAttribute>()
            ?.Name;
    }
}
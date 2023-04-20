namespace Platipus.Wallet.Api.StartupSettings.Logging;

using System.Reflection;
using global::Serilog.Core;
using global::Serilog.Events;

public class AppVersionEnricher : ILogEventEnricher
{
    private const string AppVersion = "AppVersion";

    private readonly string _assemblyVersion = Assembly.GetEntryAssembly()?.GetName().Version?.ToString()!;

    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        var property = propertyFactory.CreateProperty(AppVersion, _assemblyVersion);
        logEvent.AddPropertyIfAbsent(property);
    }
}
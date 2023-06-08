namespace Platipus.Wallet.Api.StartupSettings.Logging;

using global::Serilog;
using global::Serilog.Configuration;

public static class SerilogEnrichersExtensions
{
    public static LoggerConfiguration WithAppVersion(this LoggerEnrichmentConfiguration enrich)
        => enrich.With<AppVersionEnricher>();
}
namespace Platipus.Wallet.Api.StartupSettings.Extensions;

internal static class HostEnvironmentExtensions
{
    public static bool IsDebug(this IHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("Debug");
}
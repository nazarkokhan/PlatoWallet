namespace Platipus.Wallet.Api.StartupSettings.Extensions;

public static class HostEnvironmentExtensions
{
    public static bool IsDebug(this IHostEnvironment hostEnvironment) => hostEnvironment.IsEnvironment("Debug");
}
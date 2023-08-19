namespace Platipus.Wallet.Api;

using System.Reflection;

public static class PlatipusCommonEndpointsTest
{
    public static IEndpointConventionBuilder MapVersionTest(
        this IEndpointRouteBuilder routeBuilder,
        string pattern = "test-version")
        => routeBuilder.MapGet(
            pattern,
            () =>
            {
                var entryAssembly = Assembly.GetEntryAssembly()!;
                // var fileVersionInfo = FileVersionInfo.GetVersionInfo(entryAssembly.Location);
                // var fileVersion = fileVersionInfo.In;
                return entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()!.InformationalVersion;
            });
}
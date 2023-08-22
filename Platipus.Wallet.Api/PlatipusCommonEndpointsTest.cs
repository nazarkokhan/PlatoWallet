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
                var informationalVersion = entryAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
                return informationalVersion!.InformationalVersion;
            });
}
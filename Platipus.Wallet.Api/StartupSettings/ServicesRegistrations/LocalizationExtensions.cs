namespace Platipus.Wallet.Api.StartupSettings.ServicesRegistrations;

using System.Globalization;
using Microsoft.AspNetCore.Localization;

public static class LocalizationExtensions
{
    public static IServiceCollection AddLocalizationServices(
        this IServiceCollection services,
        string defaultCulture,
        IEnumerable<string> supportedCultures)
        => services
            .AddMemoryCache()
            // .AddPortableObjectLocalization()
            .AddLocalization(options => options.ResourcesPath = "Localization")
            .Configure<RequestLocalizationOptions>(
                options =>
                {
                    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
                    options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                });
}
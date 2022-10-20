namespace PlatipusWallet.Api.StartupSettings;

using Filters;

public static class StartupConstants
{
    public static readonly List<string> AllowedHeaders = new()
    {
        PswHeaders.XRequestSign,
        "X-Real-IP",
        "X-Forwarded-Proto",
        "X-Forwarded-For",
        "Refer",
        "Origin",
        "Referer",
        "sec-ch-ua",
        "sec-ch-ua-mobile",
        "sec-ch-ua-platform",
        "Sec-Fetch-Site",
        "Sec-Fetch-Mode",
        "Sec-Fetch-Dest"
    };
}
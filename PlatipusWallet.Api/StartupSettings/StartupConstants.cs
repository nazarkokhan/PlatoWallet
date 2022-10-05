namespace PlatipusWallet.Api.StartupSettings;

public static class StartupConstants
{
    public static readonly List<string> AllowedHeaders = new()
    {
        "X-REQUEST-SIGN",
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
public static class StartupConstants
{
    public static readonly List<string> AllowedHeaders = new List<string>
    {
        "X-REQUEST-SIGN", "X-Real-IP", "X-Forwarded-Proto", "X-Forwarded-For",
        "Refer"
    };
}
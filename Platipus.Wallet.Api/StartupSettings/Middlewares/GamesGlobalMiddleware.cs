namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

using System.Text;

public class GamesGlobalAuthMiddleware : IMiddleware
{
    private readonly ILogger<GamesGlobalAuthMiddleware> _logger;

    public GamesGlobalAuthMiddleware(ILogger<GamesGlobalAuthMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.StartsWithSegments("/wallet/games-global"))
        {
            await next(context);
            return;
        }
    }
}

public class GamesGlobalMiddleware : IMiddleware
{
    private readonly ILogger<GamesGlobalMiddleware> _logger;

    public GamesGlobalMiddleware(ILogger<GamesGlobalMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.StartsWithSegments("/wallet/games-global"))
        {
            await next(context);
            return;
        }

        var rawRequest = (byte[])context.Items[HttpContextItemKeys.RawRequestBytes]!;
        var originalRequestString = Encoding.UTF8.GetString(rawRequest);

        var modifiedRequestString = originalRequestString
            .Replace("<long>", "<i8>")
            .Replace("</long>", "</i8>");

        var requestData = Encoding.UTF8.GetBytes(modifiedRequestString);
        context.Request.Body = new MemoryStream(requestData);
        context.Request.ContentLength = context.Request.Body.Length;

        var nativeResponseStream = context.Response.Body;

        try
        {
            using var tempResponseStream = new MemoryStream();
            context.Response.Body = tempResponseStream;

            await next(context);
            tempResponseStream.Position = 0;

            var originalResponseBytes = new byte[Convert.ToInt32(tempResponseStream.Length)];
            _ = await tempResponseStream.ReadAsync(originalResponseBytes);
            var originalResponseString = Encoding.UTF8.GetString(originalResponseBytes);

            var modifiedResponseString = originalResponseString
                .Replace("<i4>", "<int>")
                .Replace("</i4>", "</int>")
                .Replace("<i8>", "<long>")
                .Replace("</i8>", "</long>");
            var modifiedResponseBytes = Encoding.UTF8.GetBytes(modifiedResponseString);

            context.Response.ContentLength = modifiedResponseBytes.Length;
            await nativeResponseStream.WriteAsync(modifiedResponseBytes);
        }
        finally
        {
            context.Response.Body = nativeResponseStream;
        }
    }
}
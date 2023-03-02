namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

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
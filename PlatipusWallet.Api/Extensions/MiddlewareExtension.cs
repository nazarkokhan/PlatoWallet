namespace PlatipusWallet.Api.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder EnableBuffering(this IApplicationBuilder app)
        => app.Use(
            async (context, next) =>
            {
                context.Request.EnableBuffering();
                await next();
            });
}
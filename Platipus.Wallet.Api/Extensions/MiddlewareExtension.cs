namespace Platipus.Wallet.Api.Extensions;

public static class MiddlewareExtension
{
    public static IApplicationBuilder EnableBufferingAndSaveRawRequest(this IApplicationBuilder app)
        => app.Use(
            async (context, next) =>
            {
                context.Request.EnableBuffering();

                var requestBytes = new byte[Convert.ToInt32(context.Request.ContentLength)];
                var requestBytes2 = new byte[Convert.ToInt32(context.Request.ContentLength)];
                _ = await context.Request.Body.ReadAsync(requestBytes);
                context.Request.Body.Position = 0;

                context.Items.Add("rawRequestBytes", requestBytes);
                await next();
            });
}
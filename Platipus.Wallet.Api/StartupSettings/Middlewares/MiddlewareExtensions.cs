namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

public static class MiddlewareExtensions
{
    public static void BufferResponseBody(this IApplicationBuilder app)
    {
        app.UseMiddleware<BufferResponseBodyMiddleware>();
    }

    public static void BufferRequestBody(this IApplicationBuilder app)
    {
        app.UseMiddleware<BufferRequestBodyMiddleware>();
    }
}
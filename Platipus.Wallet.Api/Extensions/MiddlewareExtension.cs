namespace Platipus.Wallet.Api.Extensions;

using System.Text;
using StartupSettings;

public static class MiddlewareExtension
{
    public static IApplicationBuilder EnableBufferingAndSaveRawRequest(this IApplicationBuilder app)
        => app.Use(
            async (context, next) =>
            {
                var httpRequest = context.Request;
                httpRequest.EnableBuffering();

                byte[] requestBytes;
                if (httpRequest.Method is not "GET")
                {
                    requestBytes = new byte[Convert.ToInt32(httpRequest.ContentLength)];
                    _ = await httpRequest.Body.ReadAsync(requestBytes);
                    httpRequest.Body.Position = 0;
                }
                else
                {
                    requestBytes = Encoding.UTF8.GetBytes(httpRequest.QueryString.ToString());
                }

                context.Items.Add(HttpContextItems.RequestBodyBytes, requestBytes);
                await next();
            });
}
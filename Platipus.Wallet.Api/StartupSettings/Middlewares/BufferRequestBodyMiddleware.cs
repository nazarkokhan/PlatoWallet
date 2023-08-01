namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

public class BufferRequestBodyMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var queryString = context.Request.QueryString;
        if (queryString.HasValue)
        {
            context.Items.Add(HttpContextItems.RequestQueryString, queryString.Value);
        }

        context.Request.EnableBuffering();

        if (context.Request.Method is not "GET")
        {
            var requestBytes = new byte[Convert.ToInt32(context.Request.ContentLength)];
            _ = await context.Request.Body.ReadAsync(requestBytes);
            context.Items.Add(HttpContextItems.RequestBodyBytes, requestBytes);
            context.Request.Body.Position = 0;
        }

        await next(context);
    }
}
namespace Platipus.Wallet.Api.StartupSettings.Middlewares;

public class BufferResponseBodyMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var originalBody = context.Response.Body;

        byte[] responseBytes;
        try
        {
            using var memStream = new MemoryStream();
            context.Response.Body = memStream;

            await next(context);
            memStream.Position = 0;

            responseBytes = new byte[Convert.ToInt32(memStream.Length)];
            _ = await memStream.ReadAsync(responseBytes);

            await originalBody.WriteAsync(responseBytes);
        }
        finally
        {
            context.Response.Body = originalBody;
        }

        context.Items.Add(HttpContextItems.ResponseBodyBytes, responseBytes);
    }
}
namespace Platipus.Wallet.Api.Obsolete;

using System.Text;
using StartupSettings;

public class BufferResponseBodyMiddleware : IMiddleware
{
    private readonly ILogger<BufferResponseBodyMiddleware> _logger;

    public BufferResponseBodyMiddleware(ILogger<BufferResponseBodyMiddleware> logger)
    {
        _logger = logger;
    }

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

        var str = Encoding.UTF8.GetString(responseBytes);
        _logger.LogInformation("Response body: {ResponseBodyBytes}", str);
    }
}
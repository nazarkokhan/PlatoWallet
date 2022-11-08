namespace Platipus.Wallet.Api.Obsolete;

using System.Text.Json;

[Obsolete]
public class LoggingMiddleware : IMiddleware
{
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(ILogger<LoggingMiddleware> logger)
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

        context.Request.Body.Position = 0;
        var requestBytes = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(requestBytes);
        context.Request.Body.Position = 0;

        Dictionary<string, string>? request;
        try
        {
            request = JsonSerializer.Deserialize<Dictionary<string, string>>(requestBytes);
        }
        catch
        {
            request = new Dictionary<string, string>();
        }

        Dictionary<string, string>? response;
        try
        {
            response = JsonSerializer.Deserialize<Dictionary<string, string>>(responseBytes);
        }
        catch
        {
            response = new Dictionary<string, string>();
        }

        var requestHeaders = context.Request.Headers.ToDictionary(x => x.Key, x => x.Value);
        var responseHeaders = context.Response.Headers.ToDictionary(x => x.Key, x => x.Value);

        _logger.LogInformation(
            "RequestBody: {@RequestBody} \n" +
            "ResponseBody: {@ResponseBody} \n" +
            "RequestHeaders: {@RequestHeaders} \n" +
            "ResponseHeaders: {@ResponseHeaders} \n",
            request,
            response,
            requestHeaders,
            responseHeaders);
    }
}
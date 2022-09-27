namespace PlatipusWallet.Api.StartupSettings.Middlewares;

using System.Security.Cryptography;
using System.Text;
using System.Web;

public class TestBodyHashingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path != "/test/get-hash-body")
        {
            await next(context);
            return;
        }

        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(buffer);

        var bufferString = Encoding.UTF8.GetString(buffer);

        var signatureKey = HttpUtility
            .ParseQueryString(context.Request.QueryString.Value!)
            .Get("signature_key")!;

        var signatureKeyBytes = Encoding.UTF8.GetBytes(signatureKey);

        var hmac = HMACSHA256.HashData(signatureKeyBytes, buffer);

        var validSignature = Convert.ToHexString(hmac);

        await context.Response.WriteAsJsonAsync(validSignature);
    }
}
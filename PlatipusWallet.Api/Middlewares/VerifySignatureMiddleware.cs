namespace PlatipusWallet.Api.Middlewares;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public class VerifySignatureMiddleware : IMiddleware
{
    private readonly byte[] _authToken = Encoding.UTF8.GetBytes("12345678");

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Headers.TryGetValue("X-REQUEST-SIGN", out var signature))
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            return;
        }

        context.Request.EnableBuffering();
        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(buffer);
        var ownSignature = Convert.ToBase64String(HMACSHA256.HashData(_authToken, buffer));

        if (ownSignature != signature)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;

            return;
        }

        context.Request.Body.Position = 0;

        await next(context);
    }
}
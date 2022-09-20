namespace PlatipusWallet.Api.Middlewares;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Results.External;
using Results.External.Enums;

public class VerifySignatureMiddleware : IMiddleware
{
    private readonly string _walletControllerPath = new PathString("/wallet");
    private readonly BaseResponse _errorResponse = new(Status.Ok);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.EnableBuffering();

        if (!context.Request.Path.StartsWithSegments(_walletControllerPath))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-REQUEST-SIGN", out var signature))
        {
            await MakeErrorResponse(context);
            return;
        }

        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(buffer);

        var casinoId = JsonNode.Parse(buffer)?["casino_id"]?.GetValue<string>();

        if (casinoId is null)
        {
            await MakeErrorResponse(context);
            return;
        }

        var dbContext = context.RequestServices.GetRequiredService<DbContext>();
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Id == casinoId)
            .FirstOrDefaultAsync();

        if (casino is null)
        {
            await MakeErrorResponse(context);
            return;
        }

        var signatureBytes = Encoding.UTF8.GetBytes(casino.SignatureKey);
        var ownSignature = Convert.ToBase64String(HMACSHA256.HashData(signatureBytes, buffer));

        if (ownSignature != signature)
        {
            await MakeErrorResponse(context);
            return;
        }

        context.Request.Body.Position = 0;

        await next(context);
    }

    private Task MakeErrorResponse(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
        return context.Response.WriteAsJsonAsync(_errorResponse);
    }
}
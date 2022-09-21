namespace PlatipusWallet.Api.Middlewares;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Application.Requests.Base;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Results.External.Enums;

public class VerifySignatureMiddleware : IMiddleware
{
    private readonly BaseResponse _errorResponse = new(Status.Error);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path != "/wallet")
        {
            await next(context);
            return;
        }


        if (!context.Request.Headers.TryGetValue("X-REQUEST-SIGN", out var requestSignature))
        {
            await MakeErrorResponse(context);
            return;
        }

        context.Request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(buffer);

        var sessionIdString = JsonNode.Parse(buffer)?["session_id"]?.GetValue<string>();

        if (sessionIdString is null || Guid.TryParse(sessionIdString, out var sessionId))
        {
            await MakeErrorResponse(context);
            return;
        }

        var dbContext = context.RequestServices.GetRequiredService<DbContext>();
        var session = await dbContext.Set<Session>()
            .Where(c => c.Id == sessionId)
            .Select(
                s => new
                {
                    s.Id,
                    Casion = new
                    {
                        s.User.Casino.SignatureKey
                    }
                })
            .FirstOrDefaultAsync();

        if (session is null)
        {
            await MakeErrorResponse(context);
            return;
        }

        var signatureKeyBytes = Encoding.UTF8.GetBytes(session.Casion.SignatureKey);
        var hmac = HMACSHA256.HashData(signatureKeyBytes, buffer);
        var ownSignature = Convert.ToHexString(hmac);
        
        if (ownSignature.Equals(requestSignature, StringComparison.InvariantCultureIgnoreCase))
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
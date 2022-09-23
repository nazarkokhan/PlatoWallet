namespace PlatipusWallet.Api.StartupSettings.Middlewares;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using PlatipusWallet.Api.Results.External.Enums;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.Extensions.Localization;

public class VerifySignatureMiddleware : IMiddleware
{
    private readonly IStringLocalizer<VerifySignatureMiddleware> _stringLocalizer;
    private readonly WalletDbContext _dbContext;

    public VerifySignatureMiddleware(
        IStringLocalizer<VerifySignatureMiddleware> stringLocalizer,
        WalletDbContext dbContext)
    {
        _stringLocalizer = stringLocalizer;
        _dbContext = dbContext;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.Request.Path != "/wallet")
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("X-REQUEST-SIGN", out var requestSignature))
        {
            MakeOk(context);
            const ErrorCode errorCode = ErrorCode.MissingSignature;
            var response = (Status.Error, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        context.Request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(buffer);

        var sessionIdString = JsonNode.Parse(buffer)?["session_id"]?.GetValue<string>();

        if (sessionIdString is null)
        {
            MakeOk(context);
            const ErrorCode errorCode = ErrorCode.EmptySessionId;
            var response = (Status.Error, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        if (Guid.TryParse(sessionIdString, out var sessionId))
        {
            await SessionExpired(context);
            return;
        }

        var session = await _dbContext.Set<Session>()
            .Where(c => c.Id == sessionId)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    User = new
                    {
                        s.User.IsDisabled
                    },
                    Casion = new
                    {
                        s.User.Casino.SignatureKey
                    }
                })
            .FirstOrDefaultAsync(context.RequestAborted);

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            await SessionExpired(context);
            return;
        }

        if (session.User.IsDisabled)
        {
            MakeOk(context);
            const ErrorCode errorCode = ErrorCode.UserDisabled;
            var response = (Status.Error, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        var signatureKeyBytes = Encoding.UTF8.GetBytes(session.Casion.SignatureKey);
        var hmac = HMACSHA256.HashData(signatureKeyBytes, buffer);
        var ownSignature = Convert.ToHexString(hmac);

        if (!ownSignature.Equals(requestSignature, StringComparison.InvariantCultureIgnoreCase))
        {
            MakeOk(context);
            const ErrorCode errorCode = ErrorCode.InvalidSignature;
            var response = (Status.Error, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        context.Request.Body.Position = 0;

        await next(context);
    }

    private static void MakeOk(HttpContext context)
    {
        context.Response.StatusCode = StatusCodes.Status200OK;
    }

    private static Task SessionExpired(HttpContext context)
    {
        MakeOk(context);
        const ErrorCode errorCode = ErrorCode.SessionExpired;
        var response = (Status.Error, (int) errorCode, errorCode.ToString());
        return context.Response.WriteAsJsonAsync(response);
    }
}
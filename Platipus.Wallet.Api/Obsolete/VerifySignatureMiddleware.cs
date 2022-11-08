namespace Platipus.Wallet.Api.Obsolete;

using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Application.DTOs;
using Application.Requests.Wallets.Psw.Base.Response;
using Extensions;
using LazyCache;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using Platipus.Wallet.Api.Application.Results.Psw;
using Domain.Entities;
using Infrastructure.Persistence;

[Obsolete]
public class VerifySignatureMiddleware : IMiddleware
{
    private readonly IStringLocalizer<VerifySignatureMiddleware> _stringLocalizer;
    private readonly WalletDbContext _dbContext;
    private readonly IAppCache _cache;

    public VerifySignatureMiddleware(
        IStringLocalizer<VerifySignatureMiddleware> stringLocalizer,
        WalletDbContext dbContext,
        IAppCache cache)
    {
        _stringLocalizer = stringLocalizer;
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!context.Request.Path.StartsWithSegments("/wallet/psw"))
        {
            await next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(PswHeaders.XRequestSign, out var requestSignature))
        {
            const ErrorCode errorCode = ErrorCode.MissingSignature;
            var response = new PswErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        context.Request.EnableBuffering();

        var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
        _ = await context.Request.Body.ReadAsync(buffer);

        var jsonNode = JsonNode.Parse(buffer);
        var sessionIdString = jsonNode?["session_id"]?.GetValue<string>();

        if (sessionIdString is null)
        {
            const ErrorCode errorCode = ErrorCode.EmptySessionId;
            var response = new PswErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        if (!Guid.TryParse(sessionIdString, out var sessionId))
        {
            await SessionExpired(context);
            return;
        }

        var session = await _cache.GetOrAddAsync(
            sessionIdString, async entry =>
            {
                var session = await _dbContext.Set<Session>()
                    .Where(c => c.Id == sessionId)
                    .Select(
                        s => new CachedSessionDto(
                            s.Id,
                            s.ExpirationDate,
                            s.User.IsDisabled,
                            s.User.Casino.SignatureKey))
                    .FirstOrDefaultAsync(context.RequestAborted);
                
                return session;
            }, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            });
        
        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            await SessionExpired(context);
            return;
        }

        if (session.UserIsDisabled)
        {
            const ErrorCode errorCode = ErrorCode.UserDisabled;
            var response = new PswErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        var signatureKeyBytes = Encoding.UTF8.GetBytes(session.CasinoSignatureKey);
        var hmac = HMACSHA256.HashData(signatureKeyBytes, buffer);
        var ownSignature = Convert.ToHexString(hmac);

        if (!ownSignature.Equals(requestSignature, StringComparison.InvariantCultureIgnoreCase))
        {
            const ErrorCode errorCode = ErrorCode.InvalidSignature;
            var response = new PswErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
            await context.Response.WriteAsJsonAsync(response);
            return;
        }

        context.Request.Body.Position = 0;

        await next(context);
    }

    private static async Task SessionExpired(HttpContext context)
    {
        const ErrorCode errorCode = ErrorCode.SessionExpired;
        var response = new PswErrorResponse(Status.ERROR, (int) errorCode, errorCode.ToString());
        await context.Response.WriteAsJsonAsync(response);
    }
}
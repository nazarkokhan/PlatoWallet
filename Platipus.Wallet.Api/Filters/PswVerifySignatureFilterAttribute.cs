namespace Platipus.Wallet.Api.Filters;

using System.Security.Cryptography;
using System.Text;
using Application.Requests.Base.Requests;
using Domain.Entities;
using Extensions;
using LazyCache;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Infrastructure.Persistence;
using Results.Common;
using Results.Common.Result.Factories;
using StartupSettings;
using StartupSettings.Middlewares.DTOs;

public class PswVerifySignatureFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var xRequestSign = httpContext.Request.Headers.GetXRequestSign();
        if (xRequestSign is null)
        {
            context.Result = ResultFactory.Failure(ErrorCode.MissingSignature).ToActionResult();
            return;
        }

        var sessionId = context.ActionArguments
            .Select(a => a.Value as PswBaseRequest)
            .SingleOrDefault(a => a is not null)
            ?.SessionId; //TODO condition access code style settings

        if (sessionId is null)
        {
            context.Result = ResultFactory.Failure(ErrorCode.EmptySessionId).ToActionResult();
            return;
        }

        var services = httpContext.RequestServices;
        var cache = services.GetRequiredService<IAppCache>();
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var session = await cache.GetOrAddAsync(
            sessionId.ToString(), async _ =>
            {
                var session = await dbContext.Set<Session>()
                    .Where(c => c.Id == sessionId)
                    .Select(
                        s => new CachedSessionDto(
                            s.Id,
                            s.ExpirationDate,
                            s.User.IsDisabled,
                            s.User.Casino.SignatureKey))
                    .FirstOrDefaultAsync(httpContext.RequestAborted);

                return session;
            },
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)
            });

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = ResultFactory.Failure(ErrorCode.SessionExpired).ToActionResult();
            return;
        }

        if (session.UserIsDisabled)
        {
            context.Result = ResultFactory.Failure(ErrorCode.UserDisabled).ToActionResult();
            return;
        }

        var rawRequestBytes = (byte[])httpContext.Items["rawRequestBytes"]!;

        var signatureKeyBytes = Encoding.UTF8.GetBytes(session.CasinoSignatureKey);
        var hmac = HMACSHA256.HashData(signatureKeyBytes, rawRequestBytes);
        var ownSignature = Convert.ToHexString(hmac);

        if (!ownSignature.Equals(xRequestSign, StringComparison.InvariantCultureIgnoreCase))
        {
            context.Result = ResultFactory.Failure(ErrorCode.InvalidSignature).ToActionResult();
            return;
        }

        var executedContext = await next();
    }
}
namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.DTOs;
using Application.Requests.Wallets.Psw.Base;
using Application.Results.Psw;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using LazyCache;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class Hub88VerifySignatureFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var xRequestSign = httpContext.Request.Headers.GetXHub88Signature();
        if (xRequestSign is null)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.MissingSignature).ToActionResult();
            return;
        }

        var sessionId = context.ActionArguments
            .Select(a => a.Value as PswBaseRequest)
            .SingleOrDefault(a => a is not null)
            ?.SessionId; //TODO condition access code style settings

        if (sessionId is null)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.EmptySessionId).ToActionResult();
            return;
        }

        var services = httpContext.RequestServices;
        var cache = services.GetRequiredService<IAppCache>();
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var session = await cache.GetOrAddAsync(
            sessionId.ToString(),
            async _ =>
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
            new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)});

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.SessionExpired).ToActionResult();
            return;
        }

        if (session.UserIsDisabled)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.UserDisabled).ToActionResult();
            return;
        }

        var rawRequestBytes = (byte[]) httpContext.Items["rawRequestBytes"]!;

        var isValidSign = Hub88RequestSign.IsValidSign(xRequestSign, rawRequestBytes, session.CasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = PswResultFactory.Failure(PswErrorCode.InvalidSignature).ToActionResult();
            return;
        }

        var executedContext = await next();
    }
}
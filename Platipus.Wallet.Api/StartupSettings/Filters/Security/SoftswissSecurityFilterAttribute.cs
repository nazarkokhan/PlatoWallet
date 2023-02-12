namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Application.DTOs;
using Application.Requests.Wallets.Softswiss.Base;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using LazyCache;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class SoftswissSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var xRequestSign = httpContext.Request.Headers.GetXRequestSign();
        if (xRequestSign is null)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        var sessionId = context.ActionArguments
            .Select(a => a.Value as ISoftswissBaseRequest)
            .SingleOrDefault(a => a is not null)
            ?.SessionId; //TODO condition access code style settings

        if (sessionId is null)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
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
                            s.User.Id,
                            s.User.IsDisabled,
                            s.User.Casino.SignatureKey))
                    .FirstOrDefaultAsync(httpContext.RequestAborted);

                return session;
            },
            new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10) });

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        if (session.UserIsDisabled)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.PlayerIsDisabled).ToActionResult();
            return;
        }

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();

        var isValidSign = SoftswissSecurityHash.IsValid(xRequestSign, rawRequestBytes, session.CasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        var executedContext = await next();
    }
}
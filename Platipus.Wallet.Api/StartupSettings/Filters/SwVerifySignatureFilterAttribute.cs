namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.DTOs;
using Application.Extensions;
using Application.Requests.Wallets.Sw.Base;
using Application.Results.Sw;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using LazyCache;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

public class SwVerifySignatureFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var request = context.ActionArguments
            .Select(a => a.Value as ISwBaseRequest)
            .Single(a => a is not null);

        if (request is null)
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.ExpiredToken).ToActionResult();
            return;
        }

        var sessionId = request.Token;

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
            new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10)});

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.ExpiredToken).ToActionResult();
            return;
        }

        if (session.UserIsDisabled)
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.UserNotFound).ToActionResult();
            return;
        }

        var isValidSign = request switch
        {
            ISwMd5Request md5Request => md5Request.Map(
                r => SwRequestMd5.IsValidSign(
                    r.Md5,
                    r.ProviderId,
                    r.UserId,
                    session.CasinoSignatureKey)),
            ISwHashRequest hashRequest => hashRequest.Map(
                r => SwRequestHash.IsValidSign(
                    r.Hash,
                    r.ProviderId,
                    r.UserId,
                    session.CasinoSignatureKey)),
            _ => false
        };

        if (!isValidSign)
        {
            context.Result = SwResultFactory.Failure(SwErrorCode.InvalidMd5OrHash).ToActionResult();
            return;
        }

        var executedContext = await next();
    }
}
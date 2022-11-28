namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.DTOs;
using Application.Requests.Wallets.Hub88.Base;
using Application.Results.Hub88;
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
        var request = context.ActionArguments
            .Select(a => a.Value as IHub88BaseRequest)
            .Single(a => a is not null)!;

        var httpContext = context.HttpContext;
        var cancellationToken = httpContext.RequestAborted;
        var services = httpContext.RequestServices;
        var dbContext = services.GetRequiredService<WalletDbContext>();

        var requestExist = await dbContext.Set<Request>()
            .Where(r => r.Id == request.RequestUuid)
            .AnyAsync(cancellationToken);

        if (requestExist)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_WRONG_SYNTAX).ToActionResult();
            return;
        }

        var xRequestSign = httpContext.Request.Headers.GetXHub88Signature();
        if (xRequestSign is null)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_INVALID_SIGNATURE).ToActionResult();
            return;
        }

        var sessionId = request.Token;

        var cache = services.GetRequiredService<IAppCache>();

        var session = await cache.GetOrAddAsync(
            sessionId.ToString(),
            async entry =>
            {
                var sessionToFind = new Guid((string)entry.Key);

                var session = await dbContext.Set<Session>()
                    .Where(c => c.Id == sessionToFind)
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

        if (session is null)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_INVALID_TOKEN).ToActionResult();
            return;
        }

        if (session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_TOKEN_EXPIRED).ToActionResult();
            return;
        }

        if (session.UserIsDisabled)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_USER_DISABLED).ToActionResult();
            return;
        }

        var rawRequestBytes = (byte[])httpContext.Items["rawRequestBytes"]!;

        var isValidSign = Hub88RequestSign.IsValidSign(xRequestSign, rawRequestBytes, session.CasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = Hub88ResultFactory.Failure(Hub88ErrorCode.RS_ERROR_INVALID_SIGNATURE).ToActionResult();
            return;
        }

        var requestEntity = new Request(request.RequestUuid) {UserId = session.UserId};
        dbContext.Add(requestEntity);

        await dbContext.SaveChangesAsync(cancellationToken);

        var executedContext = await next();
    }
}
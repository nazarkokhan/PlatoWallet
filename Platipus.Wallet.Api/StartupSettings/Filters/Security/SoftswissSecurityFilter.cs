namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Softswiss;
using Application.Requests.Wallets.Softswiss.Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class SoftswissSecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public SoftswissSecurityFilter(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var xRequestSign = httpContext.Request.Headers.GetXRequestSign();
        if (xRequestSign is null)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        var baseRequest = context.HttpContext.GetRequestObject();

        var sessionQuery = _dbContext.Set<Session>()
           .AsQueryable();

        if (baseRequest is SoftswissFreespinsRequest freespinsRequest)
        {
            var awardId = freespinsRequest.IssueId;
            httpContext.Items.Add(HttpContextItems.SoftswissAwardSessionId, awardId);
            sessionQuery = sessionQuery
               .Where(q => q.User.Awards.Any(a => a.Id == awardId));
        }
        else
        {
            var request = (ISoftswissBaseRequest)baseRequest;
            sessionQuery = sessionQuery
               .Where(s => s.Id == request.SessionId);
        }
        var session = await sessionQuery
           .OrderByDescending(s => s.ExpirationDate)
           .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    User = new
                    {
                        s.User.IsDisabled,
                        CasinoSignatureKey = s.User.Casino.SignatureKey
                    },
                    IsTemporary = s.IsTemporaryToken
                })
           .FirstOrDefaultAsync(httpContext.RequestAborted);

        if (session is null || (session.IsTemporary && session.ExpirationDate < DateTime.UtcNow))
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        if (session.User.IsDisabled)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.PlayerIsDisabled).ToActionResult();
            return;
        }

        var rawRequestBytes = httpContext.GetRequestBodyBytesItem();

        var isValidSign = SoftswissSecurityHash.IsValid(xRequestSign, rawRequestBytes, session.User.CasinoSignatureKey);

        if (!isValidSign)
        {
            context.Result = SoftswissResultFactory.Failure(SoftswissErrorCode.Forbidden).ToActionResult();
            return;
        }

        await next();
    }
}
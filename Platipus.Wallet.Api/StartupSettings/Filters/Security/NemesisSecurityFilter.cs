namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Application.Requests.Wallets.Nemesis.Base;
using Application.Results.Nemesis;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class NemesisSecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public NemesisSecurityFilter(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var xIntegrationToken = context.HttpContext.Request.Headers.GetXIntegrationToken();
        if (xIntegrationToken is null)
        {
            context.Result = NemesisResultFactory.Failure(NemesisErrorCode.SessionNotFound).ToActionResult();
            return;
        }

        var request = context.ActionArguments.Values
           .OfType<INemesisRequest>()
           .Single();

        var session = await _dbContext
           .Set<Session>()
           .Where(s => s.Id == request.SessionToken)
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
           .FirstOrDefaultAsync();

        if (session is null)
        {
            context.Result = NemesisResultFactory.Failure(NemesisErrorCode.TokenIsInvalid).ToActionResult();
            return;
        }
        if (xIntegrationToken != session.User.CasinoSignatureKey)
        {
            context.Result = NemesisResultFactory.Failure(NemesisErrorCode.SessionNotFound).ToActionResult();
            return;
        }
        if (session.IsTemporary && session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = NemesisResultFactory.Failure(NemesisErrorCode.SessionExpired).ToActionResult();
            return;
        }
        if (session.User.IsDisabled)
        {
            context.Result = NemesisResultFactory.Failure(NemesisErrorCode.UserNotFound).ToActionResult();
            return;
        }

        await next();
    }
}
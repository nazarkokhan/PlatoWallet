namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Betflag.Base;
using Application.Results.Betflag;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class BetflagSecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public BetflagSecurityFilter(WalletDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IBetflagRequest>()
            .Single();

        var session = await _dbContext.Set<Session>()
            .Where(
                s => s.Id == request.Key
                  && s.User.CasinoId == request.ApiName)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    s.User.Casino.SignatureKey
                })
            .FirstOrDefaultAsync();

        context.HttpContext.Items.Add(HttpContextItems.BetflagCasinoSecretKey, session?.SignatureKey);

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = BetflagResultFactory.Failure<BetflagErrorResponse>(BetflagErrorCode.SessionExpired).ToActionResult();
            return;
        }

        var isHashValid = BetflagSecurityHash.IsValid(
            request.Hash,
            request is IBetflagBetWinCancelRequest betWinCancelRequest
                ? betWinCancelRequest.TransactionId
                : request.Key,
            request.Timestamp,
            session.SignatureKey);

        if (!isHashValid)
        {
            context.Result = BetflagResultFactory.Failure<BetflagErrorResponse>(BetflagErrorCode.InvalidToken).ToActionResult();
            return;
        }

        await next();
    }
}
namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.Requests.Wallets.Betflag.Base;
using Application.Results.Betflag;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class BetflagVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var request = context.ActionArguments.Values
            .OfType<IBetflagRequest>()
            .Single();

        var session = await dbContext.Set<Session>()
            .Where(s => s.Id == new Guid(request.Key))
            .Select(
                s => new
                {
                    s.Id,
                    s.User.Casino.SignatureKey
                })
            .FirstOrDefaultAsync();

        context.HttpContext.Items.Add(HttpContextItems.BetflagCasinoSecretKey, session?.SignatureKey);

        if (session is null)
        {
            context.Result = BetflagResultFactory.Failure<BetflagErrorResponse>(BetflagErrorCode.SessionExpired).ToActionResult();
            return;
        }

        var isHashValid = BetflagRequestHash.IsValidSign(
            request.Hash,
            request.Key,
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
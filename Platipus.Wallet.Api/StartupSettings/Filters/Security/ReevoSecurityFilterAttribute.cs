namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Reevo.Base;
using Application.Results.Reevo;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class ReevoSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var request = context.ActionArguments.Values
            .OfType<ReevoSingleRequest>()
            .Single();

        var session = await dbContext.Set<Session>()
            .Where(s => s.Id == request.GameSessionId)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    UserCasino = new
                    {
                        s.User.Casino.SignatureKey,
                        CallerId = (string)s.User.Casino.Params[CasinoParams.ReevoCallerId]!,
                        CallerPassword = (string)s.User.Casino.Params[CasinoParams.ReevoCallerPassword]!
                    }
                })
            .FirstOrDefaultAsync();

        if (session is null)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.InternalError).ToActionResult();
            return;
        }

        if (session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.InternalError).ToActionResult();
            return;
        }

        var requestQueryString = httpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value);
        requestQueryString.Remove("key");
        var withoutKey = QueryString.Create(requestQueryString).ToString();

        var casino = session.UserCasino;
        var isHashValid = ReevoSecurityHash.IsValid(
            request.Key,
            withoutKey,
            casino.SignatureKey);

        if (!isHashValid)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.InternalError).ToActionResult();
            return;
        }

        if (casino.CallerId != request.CallerId && casino.CallerPassword != request.CallerPassword)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.InternalError).ToActionResult();
            return;
        }

        await next();
    }
}
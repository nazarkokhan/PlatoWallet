namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Application.Requests.Wallets.Reevo.Base;
using Application.Results.Reevo;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
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
            .OfType<IReevoRequest>()
            .Single();

        var session = await dbContext.Set<Session>()
            .Where(s => s.Id == request.GameSessionId)
            .Select(
                s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    s.User.Casino.SignatureKey
                })
            .FirstOrDefaultAsync();

        if (session is null)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.GeneralError).ToActionResult();
            return;
        }

        if (session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.GeneralError).ToActionResult();
            return;
        }

        var requestQueryString = httpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value);
        requestQueryString.Remove("key");
        var withoutKey = QueryString.Create(requestQueryString).ToString();

        var isHashValid = ReevoSecurityHash.IsValid(
            request.Key,
            withoutKey,
            session.SignatureKey);

        if (!isHashValid)
        {
            context.Result = ReevoResultFactory.Failure<ReevoErrorResponse>(ReevoErrorCode.GeneralError).ToActionResult();
            return;
        }

        await next();
    }
}
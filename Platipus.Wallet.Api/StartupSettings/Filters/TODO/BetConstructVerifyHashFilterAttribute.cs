namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.Requests.Wallets.BetConstruct.Base;
using Application.Results.BetConstruct;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;

public class BetConstructVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var dbContext = httpContext?.RequestServices.GetService<WalletDbContext>();

        var request = context.ActionArguments
            .Select(a => a.Value as IBetConstructBaseRequest)
            .SingleOrDefault(r => r is not null);

        string stringData = request.Data.ToString();

        var isHashValid = BetConstructRequestHash.IsValidSign(request.Hash, request.Time.ToString(), stringData);

        if (!isHashValid)
        {
            context.Result = BetConstructResultFactory
                .Failure<BetConstructBaseResponse>(BetConstructErrorCode.AuthenticationFailed)
                .ToActionResult();
        }

        await next();
    }
}
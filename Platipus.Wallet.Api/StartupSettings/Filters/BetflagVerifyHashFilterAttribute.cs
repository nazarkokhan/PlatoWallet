namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Application.Requests.Wallets.BetConstruct.Base;
using Application.Requests.Wallets.Betflag.Base;
using Application.Results.Betflag;
using Domain.Entities;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class BetflagVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var dbContext = httpContext?.RequestServices.GetService<WalletDbContext>();

        var request = context.ActionArguments
            .Select(a => a.Value as IBetflagBaseRequest)
            .SingleOrDefault(r => r is not null);

        var session = await dbContext.Set<Session>().FirstOrDefaultAsync(s => s.Id == new Guid(request.Key));

        var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var isHashValid = BetflagRequestHash.IsValidSign(request.Hash, session.UserId.ToString(), timeStamp);

        if (!isHashValid)
        {
            context.Result = BetflagResultFactory
                .Failure<BetflagErrorResponse>(BetflagErrorCode.InvalidToken, new Exception("Invalid Hash"))
                .ToActionResult();
        }

        await next();
    }
}
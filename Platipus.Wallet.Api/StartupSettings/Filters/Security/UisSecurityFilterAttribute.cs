namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Application.Requests.Wallets.Uis.Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public class UisSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var baseRequest = context.ActionArguments.Values.OfType<IUisHashRequest>().Single();

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();
        var casino = await dbContext.Set<Casino>()
            .Where(c => c.Provider == CasinoProvider.Uis)
            .Select(c => new { c.SignatureKey })
            .FirstAsync(context.HttpContext.RequestAborted);

        var isValidSecurity = baseRequest.IsValid(casino.SignatureKey);

        if (!isValidSecurity)
        {
            context.Result = DafabetResultFactory.Failure(DafabetErrorCode.InvalidHash).ToActionResult();
            return;
        }

        await next();
    }
}
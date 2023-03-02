namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Dafabet.Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

public class DatabetSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IDafabetRequest>()
            .Single();

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .Single()
            .Template!;

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var user = await dbContext.Set<User>()
            .Where(u => u.Username == request.PlayerId)
            .Select(u => new { CasinoSignatureKey = u.Casino.SignatureKey })
            .FirstOrDefaultAsync(context.HttpContext.RequestAborted);

        if (user is null)
        {
            context.Result = DafabetResultFactory.Failure(DafabetErrorCode.PlayerNotFound).ToActionResult();
            return;
        }

        var isValidJSysHash = request.IsValid(requestRoute, user.CasinoSignatureKey);

        if (!isValidJSysHash)
        {
            context.Result = DafabetResultFactory.Failure(DafabetErrorCode.InvalidHash).ToActionResult();
            return;
        }

        await next();
    }
}
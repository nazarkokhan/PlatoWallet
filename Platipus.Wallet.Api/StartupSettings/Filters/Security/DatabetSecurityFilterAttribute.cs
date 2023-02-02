namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Application.Requests.Wallets.Dafabet.Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Extensions.SecuritySign;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;

public class DatabetSecurityFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var baseRequest = context.ActionArguments.Values.OfType<IDafabetBaseRequest>().Single();

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .SingleOrDefault()
            ?.Template;

        if (requestRoute is null)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DatabetSecurityFilterAttribute>>();
            logger.LogCritical("Dafabet wallet request route not found for endpoint");
        }

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();
        var databetCasino = await dbContext.Set<Casino>()
            .Where(c => c.Provider == CasinoProvider.Dafabet)
            .Select(c => new { c.SignatureKey })
            .FirstAsync(context.HttpContext.RequestAborted);

        var secretKey = databetCasino.SignatureKey;

        var isValidJSysHash = baseRequest.IsValid(requestRoute ?? string.Empty, secretKey);

        if (!isValidJSysHash)
        {
            context.Result = DafabetResultFactory.Failure(DafabetErrorCode.InvalidHash).ToActionResult();
            return;
        }

        await next();
    }
}
namespace PlatipusWallet.Api.Filters;

using Application.Requests.Base.Requests;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result.Factories;

public class DatabetVerifySignatureFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var baseRequest = context.ActionArguments.Select(a => a.Value as DatabetBaseRequest).SingleOrDefault(a => a is not null);

        if (baseRequest is null)
        {
            context.Result = DatabetResultFactory.Failure(DatabetErrorCode.SystemError).ToActionResult();
            return;
        }

        var requestRoute = context.ActionDescriptor.EndpointMetadata
            .OfType<HttpMethodAttribute>()
            .SingleOrDefault()?
            .Template;

        if (requestRoute is null)
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<DatabetVerifySignatureFilterAttribute>>();
            logger.LogCritical("Dafabet wallet request route not found for endpoint");
        }

        var source = $"{requestRoute}{baseRequest.GetSource()}";

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();
        var databetCasino = await dbContext.Set<Casino>()
            .Where(c => c.Provider == CasinoProvider.Databet)
            .Select(
                c => new
                {
                    c.SignatureKey
                })
            .FirstAsync(context.HttpContext.RequestAborted);

        var secretKey = databetCasino.SignatureKey;

        var isValidJSysHash = baseRequest.IsValidDatabetHash(source, secretKey);

        if (!isValidJSysHash)
        {
            context.Result = DatabetResultFactory.Failure(DatabetErrorCode.InvalidHash).ToActionResult();
            return;
        }

        var executedContext = await next();
    }
}
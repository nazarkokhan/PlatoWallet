namespace PlatipusWallet.Api.Filters;

using Application.Requests.Base.Requests;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
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

        var requestPath = context.HttpContext.Request.Path;

        var contextRouteData = context.RouteData;
        var source = $"{requestPath}{baseRequest.GetSource()}";
        
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var secretKey = "secret";
        // var databetCasino = await dbContext.Set<Casino>() //TODO
        //     .Where(c => c.Id == "databetCasino")
        //     .Select(c => new
        //     {
        //         c.SignatureKey
        //     })
        //     .FirstOrDefaultAsync(context.HttpContext.RequestAborted);
        //
        // if (databetCasino is null)
        // {
        //     context.Result = DatabetResultFactory.Failure(DatabetErrorCode.SystemError).ToActionResult();
        //     return;
        // }
        // secretKey = databetCasino.SignatureKey;
        
        var isValidJSysHash = baseRequest.IsValidDatabetHash(source, secretKey);

        if (!isValidJSysHash)
        {
            context.Result = DatabetResultFactory.Failure(DatabetErrorCode.SystemError).ToActionResult();
            return;
        }
        
        var executedContext = await next();
    }
}
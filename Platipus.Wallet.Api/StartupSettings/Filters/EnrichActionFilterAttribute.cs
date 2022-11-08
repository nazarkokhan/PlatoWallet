namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

public class EnrichActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<EnrichActionFilterAttribute>>();
        using (logger.BeginScope("Start logging provider {ScopeProvider}", context.Controller.GetTypeName()))
        {
            var executedContext = await next();
        }
    }
}
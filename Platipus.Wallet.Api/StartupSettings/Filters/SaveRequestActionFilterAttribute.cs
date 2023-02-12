namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

public class SaveRequestActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var hasRequestBody = context.ActionArguments.TryGetValue(HttpContextItems.RequestObject, out var request);

        if (hasRequestBody)
            context.HttpContext.Items.Add(HttpContextItems.RequestObject, request);
    }
}
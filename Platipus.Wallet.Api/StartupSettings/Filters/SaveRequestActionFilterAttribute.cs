namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

public class SaveRequestActionFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        const string requestParamName = "request";
        var hasRequestBody = context.ActionArguments.TryGetValue(requestParamName, out var request);
        // && context.HttpContext.Request.Method is not "GET"

        if (hasRequestBody)
            context.HttpContext.Items.Add(requestParamName, request);
    }
}
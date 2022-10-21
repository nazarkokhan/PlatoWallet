namespace PlatipusWallet.Api.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

public class SaveRequestFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.ActionArguments
            .SingleOrDefault(a => a.Key is "request");

        context.HttpContext.Items.Add(request.Key, request.Value);
    }
}
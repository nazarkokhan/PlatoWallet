namespace PlatipusWallet.Api.Filters;

using Microsoft.AspNetCore.Mvc.Filters;

public class SignatureValidationActionFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();
    }
}
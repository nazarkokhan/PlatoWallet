namespace Platipus.Wallet.Api.StartupSettings.Filters;

using Castle.Core.Internal;
using ControllerSpecificJsonOptions;
using Microsoft.AspNetCore.Mvc.Filters;

public class SaveRequestDataFilterAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var request = context.ActionArguments.Values
           .OfType<IBaseRequest>()
           .SingleOrDefault();
        context.HttpContext.Items.Add(HttpContextItems.RequestObject, request);

        var provider = context.Controller
           .GetType()
           .GetAttribute<JsonSettingsNameAttribute>()
          ?.Type;

        context.HttpContext.Items.Add(HttpContextItems.Provider, provider);
    }
}
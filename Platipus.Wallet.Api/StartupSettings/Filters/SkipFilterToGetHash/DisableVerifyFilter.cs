// namespace Platipus.Wallet.Api.StartupSettings.Filters.SkipFilterToGetHash;
//
// using Api.Extensions;
// using Microsoft.AspNetCore.Mvc.Filters;
//
// public class DisableVerifyFilter : ActionFilterAttribute
// {
//     private readonly string _filterToDisable;
//     private string _getTypeName;
//
//     public DisableVerifyFilter(string filterToDisable, int order)
//     {
//         Order = order;
//         _filterToDisable = filterToDisable;
//     }
//
//     public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
//     {
//         var filtertoremove = context.Filters.FirstOrDefault(f => f.GetTypeName() == _filterToDisable);
//
//         context.Filters.(filtertoremove);
//
//         await next();
//     }
// }
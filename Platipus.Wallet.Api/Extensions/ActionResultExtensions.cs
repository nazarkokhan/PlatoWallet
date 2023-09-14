namespace Platipus.Wallet.Api.Extensions;

using Application.Results.Base;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ActionResults;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this IBaseResult result) => new BaseExternalActionResult(result);

    public static IActionResult ToActionResult(this IDafabetResult result) => new DafabetExternalActionResult(result);

    public static IActionResult ToActionResult(this IOpenboxResult result) => new OpenboxExternalActionResult(result);
}
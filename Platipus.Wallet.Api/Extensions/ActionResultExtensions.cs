namespace Platipus.Wallet.Api.Extensions;

using Application.Results.Base;
using Application.Results.Hub88;
using Application.Results.PariMatch;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ActionResults;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this IBaseResult result) => new BaseExternalActionResult(result);

    public static IActionResult ToActionResult(this IResult result) => new ExternalActionResult(result);

    public static IActionResult ToActionResult(this IPswResult result) => new PswExternalActionResult(result);

    public static IActionResult ToActionResult(this IDafabetResult result) => new DafabetExternalActionResult(result);

    public static IActionResult ToActionResult(this IOpenboxResult result) => new OpenboxExternalActionResult(result);

    public static IActionResult ToActionResult(this IHub88Result result) => new Hub88ExternalActionResult(result);

    public static IActionResult ToActionResult(this ISoftswissResult result) => new SoftswissExternalActionResult(result);

    public static IActionResult ToActionResult(this IPariMatchResult result) => new PariMatchExternalActionResult(result);
}
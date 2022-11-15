namespace Platipus.Wallet.Api.Extensions;

using Application.Results.Dafabet;
using Application.Results.Openbox;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ActionResults;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this IResult result)
        => new ExternalActionResult(result);

    public static IActionResult ToActionResult(this IPswResult result)
        => new PswExternalActionResult(result);

    public static IActionResult ToActionResult(this IDafabetResult result)
        => new DafabetExternalActionResult(result);

    public static IActionResult ToActionResult(this IOpenboxResult result)
        => new OpenboxExternalActionResult(result);

    // public static IActionResult ToActionResult(this IOpenboxResult result) => new OpenboxExternalActionResult(result);
}
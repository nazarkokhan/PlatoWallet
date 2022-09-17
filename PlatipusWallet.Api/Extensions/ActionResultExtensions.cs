namespace PlatipusWallet.Api.Extensions;

using Microsoft.AspNetCore.Mvc;
using Results.Common.Result.WithData;
using Results.External.ActionResults;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this IResult result) => new ExternalActionResult(result);

    public static IActionResult ToActionResult<T>(this IResult<T> result) => new ExternalActionResult(result);
}
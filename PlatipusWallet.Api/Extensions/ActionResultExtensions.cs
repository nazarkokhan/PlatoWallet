namespace PlatipusWallet.Api.Extensions;

using Microsoft.AspNetCore.Mvc;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Results.External.ActionResults;

public static class ActionResultExtensions
{
    // public static IActionResult ToActionResult<TError>(this IBaseResult<TError> result) => new ExternalActionResult<TError>(result);
    //
    // public static IActionResult ToActionResult<TError, TData>(this IBaseResult<TError, TData> result) => new ExternalActionResult<TError>(result);
    
    public static IActionResult ToActionResult(this IResult result) => new ExternalActionResult(result);

    public static IActionResult ToActionResult<TData>(this IResult<TData> result) => new ExternalActionResult(result);
    
    public static IActionResult ToActionResult(this IDatabetResult result) => new ExternalActionDatabetResult(result);

    public static IActionResult ToActionResult<TData>(this IDatabetResult<TData> result) => new ExternalActionDatabetResult(result);
}
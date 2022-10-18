namespace PlatipusWallet.Api.Extensions;

using Microsoft.AspNetCore.Mvc;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Results.External.ActionResults;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult<TError>(this IBaseResult<TError> result) => new ExternalActionResult<TError>(result);

    public static IActionResult ToActionResult<TError, TData>(this IBaseResult<TError, TData> result) => new ExternalActionResult<TError>(result);
}
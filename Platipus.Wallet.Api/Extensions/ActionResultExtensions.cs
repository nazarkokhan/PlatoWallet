namespace Platipus.Wallet.Api.Extensions;

using Application.Results.Dafabet;
using Application.Results.Dafabet.WithData;
using Application.Results.Openbox;
using Application.Results.Openbox.WithData;
using Application.Results.Psw;
using Application.Results.Psw.WithData;
using Microsoft.AspNetCore.Mvc;
using StartupSettings.ActionResults;

public static class ActionResultExtensions
{
    public static IActionResult ToActionResult(this IResult result) => new ExternalActionResult(result);
    public static IActionResult ToActionResult<TData>(this IResult<TData> result) => new ExternalActionResult(result);
    
    public static IActionResult ToActionResult(this IDafabetResult result) => new DafabetExternalActionResult(result);
    public static IActionResult ToActionResult<TData>(this IDafabetResult<TData> result) => new DafabetExternalActionResult(result);
    
    public static IActionResult ToActionResult(this IOpenboxResult result) => new OpenboxExternalActionResult(result);
    public static IActionResult ToActionResult<TData>(this IOpenboxResult<TData> result) => new OpenboxExternalActionResult(result);
}
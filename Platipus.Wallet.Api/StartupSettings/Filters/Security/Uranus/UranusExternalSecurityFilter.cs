namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.Uranus;

using System.Text;
using Api.Extensions;
using Api.Extensions.SecuritySign.Uranus;
using Application.Requests.Wallets.Uranus.Base;
using Application.Results.Uranus;
using Microsoft.AspNetCore.Mvc.Filters;

public sealed class UranusExternalSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        const string secretKey = "67dqGQzHC4ue86Kb";

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        var jsonString = Encoding.UTF8.GetString(requestBytesToValidate);

        var xSignature = context.HttpContext.Request.Headers.GetXSignature();
        if (xSignature is null)
        {
            context.Result = UranusResultFactory.Failure<UranusFailureResponse>(UranusErrorCode.E_PLAYER_SESSION_NOT_FOUND)
               .ToActionResult();

            return;
        }

        var result = UranusSecurityHash.IsValid(xSignature, jsonString, secretKey);

        if (!result)
        {
            context.Result = UranusResultFactory.Failure<UranusFailureResponse>(
                    UranusErrorCode.E_SESSION_TOKEN_INVALID_OR_EXPIRED)
               .ToActionResult();

            return;
        }

        await next();
    }
}
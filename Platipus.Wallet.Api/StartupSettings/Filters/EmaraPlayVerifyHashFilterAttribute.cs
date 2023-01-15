namespace Platipus.Wallet.Api.StartupSettings.Filters;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.Requests.Wallets.EmaraPlay.Base;
using Application.Results.EmaraPlay;
using Domain.Entities;
using Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

public class EmaraPlayVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string authHeader = context.HttpContext.Request.Headers["Authorization"];

        authHeader = authHeader.Replace("Bearer ", "");

        var rawRequestBytes = (byte[]) context.HttpContext.Items["rawRequestBytes"]!;

        var secretBytes = Encoding.UTF8.GetBytes("EmaraPlaySecret");

        var hashFromRequest = HMACSHA512.HashData(secretBytes, rawRequestBytes);

        var validSignature = Convert.ToHexString(hashFromRequest);

        var result = validSignature.Equals(authHeader);

        if (!result)
        {
            context.Result = EmaraPlayResultFactory.Failure(EmaraPlayErrorCode.InvalidHashCode).ToActionResult();
        }

        var executedContext = await next();
    }
}
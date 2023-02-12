namespace Platipus.Wallet.Api.StartupSettings.Filters.TODO;

using System.Security.Cryptography;
using System.Text;
using Application.Results.EmaraPlay;
using Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

public class EmaraPlayVerifyHashFilterAttribute : ActionFilterAttribute
{
    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        string authHeader = context.HttpContext.Request.Headers["Authorization"];

        authHeader = authHeader.Replace("Bearer ", "");

        var rawRequestBytes = context.HttpContext.GetRequestBodyBytesItem();

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
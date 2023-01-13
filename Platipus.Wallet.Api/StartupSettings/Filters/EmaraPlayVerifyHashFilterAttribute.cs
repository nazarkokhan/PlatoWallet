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
        var authHeaderBytes = Encoding.UTF8.GetBytes(authHeader);

        var request = context.ActionArguments.
            Select(a => a.Value as IEmaraPlayBaseRequest).
            SingleOrDefault(a => a is not null);

        var bodyToHash = JsonConvert.SerializeObject(request);
        var hashedBody = Encoding.UTF8.GetBytes(bodyToHash);
        var secretBytes = Encoding.UTF8.GetBytes("EmaraPlaySecret");

       var validHash = HMACSHA512.HashData(secretBytes, hashedBody);


      var result = validHash.Equals(authHeaderBytes);

      if (!result)
      {
          context.Result = EmaraPlayResultFactory.Failure(EmaraPlayErrorCode.InvalidHashCode).ToActionResult();
      }
      var executedContext = await next();
    }
}
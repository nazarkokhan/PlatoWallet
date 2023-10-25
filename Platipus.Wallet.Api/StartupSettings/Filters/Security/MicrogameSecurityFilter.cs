namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using System.Text;
using Api.Extensions;
using Application.Responses.Microgame.Base;
using Application.Results.Microgame;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;

public sealed class MicrogameSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();
        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        var jsonBody = Encoding.UTF8.GetString(requestBytesToValidate);
        var jsonObject = JObject.Parse(jsonBody);
        if (!jsonObject.TryGetValue("accessToken", out var incomingAccessToken))
        {
            context.Result = MicrogameResultFactory
               .Failure<MicrogameErrorResponse>(MicrogameStatusCode.INVALIDACCESSTOKEN)
               .ToActionResult();

            return;
        }

        var existWithIncomingToken = await dbContext.Set<Casino>()
           .AnyAsync(x => x.SignatureKey == incomingAccessToken.ToString());

        if (!existWithIncomingToken)
        {
            context.Result = MicrogameResultFactory
               .Failure<MicrogameErrorResponse>(MicrogameStatusCode.INVALIDACCESSTOKEN)
               .ToActionResult();

            return;
        }

        await next();
    }
}
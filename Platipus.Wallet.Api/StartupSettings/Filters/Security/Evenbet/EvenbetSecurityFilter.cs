namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.Evenbet;

using System.Text;
using Api.Extensions;
using Api.Extensions.SecuritySign.Evenbet;
using Application.Requests.Wallets.Evenbet.Base;
using Application.Responses.Evenbet.Base;
using Application.Results.Evenbet;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class EvenbetSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
           .OfType<IEvenbetRequest>()
           .Single();

        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();
        var query = dbContext.Set<Session>()
           .Where(s => s.Id == request.Token);

        var session = await query
           .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    UsedId = s.User.Id,
                    UserPassword = s.User.Password,
                    CasinoProvider = s.User.Casino.Params.UranusProvider
                })
           .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = EvenbetResultFactory
               .Failure<EvenbetFailureResponse>(EvenbetErrorCode.AUTHORIZATION_FAILED)
               .ToActionResult();

            return;
        }

        const int evenbetProviderId = (int)CasinoProvider.Evenbet;
        if (!session.CasinoProvider.Equals(evenbetProviderId.ToString()))
        {
            context.Result = EvenbetResultFactory
               .Failure<EvenbetFailureResponse>(EvenbetErrorCode.INVALID_PARAMETER)
               .ToActionResult();

            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        var jsonString = Encoding.UTF8.GetString(requestBytesToValidate);

        string? authHeaderValue = context.HttpContext.Request.Headers["Authorization"];
        if (authHeaderValue is null)
        {
            context.Result = EvenbetResultFactory.Failure<EvenbetFailureResponse>(EvenbetErrorCode.INVALID_TOKEN)
               .ToActionResult();

            return;
        }

        var result = EvenbetSecurityHash.IsValid(authHeaderValue, jsonString, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = EvenbetResultFactory.Failure<EvenbetFailureResponse>(EvenbetErrorCode.AUTHORIZATION_FAILED)
               .ToActionResult();

            return;
        }

        await next();
    }
}
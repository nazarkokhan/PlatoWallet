namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.Uranus;

using System.Text;
using Api.Extensions.SecuritySign.Uranus;
using Application.Requests.Wallets.Uranus.Base;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.Uranus;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;

public sealed class UranusSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
           .OfType<IUranusRequest>()
           .Single();

        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();
        IQueryable<Session> query = dbContext.Set<Session>();
        query = !string.IsNullOrEmpty(request.SessionToken)
            ? query.Where(s => s.Id == request.SessionToken)
            : query.Where(s => s.UserId.ToString() == request.PlayerId);

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
            context.Result = UranusResultFactory
               .Failure<UranusFailureResponse>(UranusErrorCode.E_SESSION_TOKEN_INVALID_OR_EXPIRED)
               .ToActionResult();

            return;
        }

        const int uranusProviderId = (int)CasinoProvider.Uranus;
        if (!session.CasinoProvider.Equals(uranusProviderId.ToString()))
        {
            context.Result = UranusResultFactory
               .Failure<UranusFailureResponse>(UranusErrorCode.E_PROVIDER_NOT_FOUND)
               .ToActionResult();

            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        var jsonString = Encoding.UTF8.GetString(requestBytesToValidate);

        string? authHeaderValue = context.HttpContext.Request.Headers["X-Signature"];
        if (authHeaderValue is null)
        {
            context.Result = UranusResultFactory.Failure<UranusFailureResponse>(UranusErrorCode.E_PLAYER_SESSION_NOT_FOUND)
               .ToActionResult();

            return;
        }

        var result = UranusSecurityHash.IsValid(authHeaderValue, jsonString, session.CasinoSignatureKey);

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
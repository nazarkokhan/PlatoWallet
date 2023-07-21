namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using System.Text;
using System.Text.Json;
using Api.Extensions;
using Application.Requests.Wallets.Anakatech.Base;
using Application.Responses.Anakatech.Base;
using Application.Results.Anakatech;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class AnakatechSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
           .OfType<IAnakatechBaseRequest>()
           .Single();

        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
           .Set<Session>()
           .Where(s => s.Id == request.SessionId)
           .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    UsedId = s.User.Id,
                    UserPassword = s.User.Password,
                    CasinoProvider = s.User.Casino.Params.AnakatechProvider
                })
           .FirstOrDefaultAsync();

        if (session?.CasinoProvider is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = AnakatechResultFactory
               .Failure<AnakatechErrorResponse>(AnakatechErrorCode.InvalidPlayerIdOrSessionId)
               .ToActionResult();

            return;
        }

        const int uranusProviderId = (int)CasinoProvider.Anakatech;
        if (!session.CasinoProvider.Equals(uranusProviderId.ToString()))
        {
            context.Result = AnakatechResultFactory
               .Failure<AnakatechErrorResponse>(AnakatechErrorCode.InvalidArgument)
               .ToActionResult();

            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        var jsonString = Encoding.UTF8.GetString(requestBytesToValidate);
        var jsonDoc = JsonDocument.Parse(jsonString);
        if (!jsonDoc.RootElement.TryGetProperty("secret", out var secretKeyElement))
        {
            context.Result = AnakatechResultFactory
               .Failure<AnakatechErrorResponse>(AnakatechErrorCode.InvalidSecret)
               .ToActionResult();

            return;
        }

        var secret = secretKeyElement.GetString();
        if (!string.Equals(secret, session.CasinoSignatureKey))
        {
            context.Result = AnakatechResultFactory
               .Failure<AnakatechErrorResponse>(AnakatechErrorCode.InvalidSecret)
               .ToActionResult();

            return;
        }
        await next();
    }
}
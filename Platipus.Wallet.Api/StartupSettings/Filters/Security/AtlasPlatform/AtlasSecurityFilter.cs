namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.AtlasPlatform;

using Api.Extensions.SecuritySign.Atlas;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities;
using Infrastructure.Persistence;
using Application.Requests.Wallets.Atlas.Base;
using Application.Results.Atlas;
using Domain.Entities.Enums;

public sealed class AtlasSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IAtlasRequest>()
            .Single();
        
        var httpContext = context.HttpContext;

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == request.Token)
            .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    UsedId = s.User.Id,
                    UserPassword = s.User.Password,
                    CasinoProvider = s.User.Casino.Params.AtlasProvider
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = AtlasResultFactory
                .Failure<AtlasErrorResponse>(AtlasErrorCode.SessionValidationFailed)
                .ToActionResult();
            return;
        }
        
        const int atlasProviderId = (int)CasinoProvider.Atlas; 
        if (!session.CasinoProvider.Equals(atlasProviderId.ToString()))
        {
            context.Result = AtlasResultFactory
                .Failure<AtlasErrorResponse>(AtlasErrorCode.ProviderNotConfigured)
                .ToActionResult();
            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        string? authHeaderValue = context.HttpContext.Request.Headers["Hash"];
        if (authHeaderValue is null)
        {
            context.Result = AtlasResultFactory.Failure<AtlasErrorResponse>(
                AtlasErrorCode.RequiredHeaderHashNotPresent).ToActionResult();
            return;
        }
        var result = AtlasSecurityHash.IsValid(
            authHeaderValue, requestBytesToValidate, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = AtlasResultFactory.Failure<AtlasErrorResponse>(
                AtlasErrorCode.HashValidationFailed).ToActionResult();
            return;
        }

        await next();
    }
}
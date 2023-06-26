namespace Platipus.Wallet.Api.StartupSettings.Filters.Security.AtlasPlatform;

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign.AtlasPlatform;
using Domain.Entities;
using Infrastructure.Persistence;
using Application.Requests.Wallets.Atlas.Base;
using Application.Results.Atlas;
using Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;

public sealed class AtlasSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor
            {
                ActionName: nameof(WalletAtlasController.Authorize)
            })
        {
            await next();
            return;
        }
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
                    UserPassword = s.User.Password
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = AtlasResultFactory
                .Failure<AtlasErrorResponse>(AtlasErrorCode.SessionValidationFailed)
                .ToActionResult();
            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        string authHeader = context.HttpContext.Request.Headers["Hash"]!;
        var result = AtlasSecurityHash.IsValid(
            authHeader, requestBytesToValidate, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = AtlasResultFactory.Failure(
                AtlasErrorCode.HashValidationFailed).ToActionResult();
            return;
        }

        await next();
    }
}
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.Application.Results.AtlasPlatform;
using Platipus.Wallet.Api.Controllers;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign.AtlasPlatform;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

public sealed class AtlasPlatformMainSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ActionDescriptor is ControllerActionDescriptor
            {
                ActionName: nameof(WalletAtlasPlatformController.Authorize)
            })
        {
            await next();
            return;
        }
        var request = context.ActionArguments.Values
            .OfType<IAtlasPlatformRequest>()
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
            context.Result = AtlasPlatformResultFactory
                .Failure<AtlasPlatformErrorResponse>(AtlasPlatformErrorCode.SessionValidationFailed)
                .ToActionResult();
            return;
        }

        string authHeader;
        bool result;
        if (context.ActionDescriptor is ControllerActionDescriptor
            {
                ActionName: nameof(WalletAtlasPlatformController.GetGames)
            })
        {
            var credentials = $"{session.UsedId}:{session.UserPassword}";
            authHeader = context.HttpContext.Request.Headers["Authorization"]!;
            var encryptedClientData = authHeader.Replace("Basic", "");
            result = AtlasPlatformSecurityBase64.IsValid(
                encryptedClientData, credentials);
        }
        else
        {
            var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
            authHeader = context.HttpContext.Request.Headers["Hash"]!;
            result = AtlasPlatformSecurityHash.IsValid(
                authHeader, requestBytesToValidate, session.CasinoSignatureKey);
        }


        if (!result)
        {
            context.Result = AtlasPlatformResultFactory.Failure(
                AtlasPlatformErrorCode.HashValidationFailed).ToActionResult();
            return;
        }

        await next();
    }
}
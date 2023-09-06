namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using System.Text;
using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Synot.Base;
using Application.Responses.Synot.Base;
using Application.Results.Synot;
using Constants.Synot;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class SynotSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var httpContext = context.HttpContext;
        var headers = httpContext.Request.Headers;

        if (!headers.TryGetValue(SynotConstants.XEasToken, out var xEasToken))
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.INVALID_TOKEN)
               .ToActionResult();

            return;
        }

        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        if (!headers.TryGetValue(SynotConstants.XEasApiKey, out var xEasApiKey))
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.INVALID_API_KEY)
               .ToActionResult();

            return;
        }

        var query = dbContext.Set<Session>()
           .Where(s => s.Id == xEasToken.ToString());

        var session = await query
           .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    UsedId = s.User.Id,
                    UserPassword = s.User.Password,
                    CasinoProvider = s.User.Casino.Params.SynotProvider,
                    ApiKey = s.User.Casino.Params.SynotApiKey,
                    IsTemporary = s.IsTemporaryToken
                })
           .FirstOrDefaultAsync();

        if (session is null)
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.INVALID_TOKEN)
               .ToActionResult();

            return;
        }
        if (session.ApiKey is null)
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.INVALID_API_KEY)
               .ToActionResult();

            return;
        }

        var validApiKey = session.ApiKey;
        if (!string.Equals(validApiKey, xEasApiKey))
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.INVALID_API_KEY)
               .ToActionResult();

            return;
        }

        if (session.IsTemporary && session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.INVALID_TOKEN)
               .ToActionResult();

            return;
        }

        const int synotProviderId = (int)WalletProvider.Synot;
        if (!string.Equals(session.CasinoProvider, synotProviderId.ToString()))
        {
            context.Result = SynotResultFactory
               .Failure<SynotErrorResponse>(SynotError.BAD_SIGNATURE)
               .ToActionResult();

            return;
        }

        if (!headers.TryGetValue(SynotConstants.XEasSignature, out var xEasSignature))
        {
            context.Result = SynotResultFactory.Failure<SynotErrorResponse>(SynotError.BAD_SIGNATURE)
               .ToActionResult();

            return;
        }

        bool isSignatureValid;
        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();
        if (requestBytesToValidate.Length is 0)
        {
            isSignatureValid = SynotSecurityHash.IsValid(
                xEasSignature!,
                null,
                session.CasinoSignatureKey,
                xEasToken!);
        }
        else
        {
            var jsonBody = Encoding.UTF8.GetString(requestBytesToValidate);

            isSignatureValid = SynotSecurityHash.IsValid(
                xEasSignature!,
                jsonBody,
                session.CasinoSignatureKey,
                xEasToken!);
        }

        if (!isSignatureValid)
        {
            context.Result = SynotResultFactory.Failure<SynotErrorResponse>(SynotError.BAD_SIGNATURE)
               .ToActionResult();

            return;
        }

        await next();
    }
}
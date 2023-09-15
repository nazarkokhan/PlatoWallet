namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Api.Extensions.SecuritySign;
using Application.Requests.Wallets.Vegangster.Base;
using Application.Responses.Vegangster.Base;
using Application.Results.Vegangster;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class VegangsterSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var xApiSignature = context.HttpContext.Request.Headers.GetXApiSignature();
        if (xApiSignature is null)
        {
            context.Result =
                VegangsterResultFactory.Failure<VegangsterFailureResponse>(VegangsterResponseStatus.ERROR_INVALID_SIGNATURE)
                   .ToActionResult();

            return;
        }

        var request = context.ActionArguments.Values
           .OfType<IVegangsterRequest>()
           .Single();

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();

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
                    CasinoProvider = s.User.Casino.Params.VegangsterProvider,
                    IsTemporary = s.IsTemporaryToken
                })
           .FirstOrDefaultAsync();

        if (session is null || (session.IsTemporary && session.ExpirationDate < DateTime.UtcNow))
        {
            context.Result = VegangsterResultFactory
               .Failure<VegangsterFailureResponse>(VegangsterResponseStatus.ERROR_INVALID_TOKEN)
               .ToActionResult();

            return;
        }

        const int vegangsterProviderId = (int)WalletProvider.Vegangster;
        if (!session.CasinoProvider.Equals(vegangsterProviderId.ToString()))
        {
            context.Result = VegangsterResultFactory
               .Failure<VegangsterFailureResponse>(VegangsterResponseStatus.ERROR_GENERAL)
               .ToActionResult();

            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();

        var isValid = VegangsterSecuritySign.IsValid(xApiSignature, requestBytesToValidate, session.CasinoSignatureKey);

        if (!isValid)
        {
            context.Result = VegangsterResultFactory.Failure<VegangsterFailureResponse>(
                    VegangsterResponseStatus.ERROR_INVALID_SIGNATURE)
               .ToActionResult();

            return;
        }

        await next();
    }
}
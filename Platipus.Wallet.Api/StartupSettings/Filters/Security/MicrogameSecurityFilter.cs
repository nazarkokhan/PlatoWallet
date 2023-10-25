namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

using Api.Extensions;
using Application.Requests.Wallets.Microgame.Base;
using Application.Responses.Microgame.Base;
using Application.Results.Microgame;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

public sealed class MicrogameSecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
           .OfType<IMicrogameBaseRequest>()
           .Single();

        var dbContext = context.HttpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
           .Set<Session>()
           .Where(s => s.Id == request.GameId)
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
            context.Result = MicrogameResultFactory
               .Failure<MicrogameErrorResponse>(MicrogameStatusCode.GENERICERROR)
               .ToActionResult();

            return;
        }

        const int vegangsterProviderId = (int)WalletProvider.Vegangster;
        if (!session.CasinoProvider.Equals(vegangsterProviderId.ToString()))
        {
            context.Result = MicrogameResultFactory
               .Failure<MicrogameErrorResponse>(MicrogameStatusCode.GENERICERROR)
               .ToActionResult();

            return;
        }

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();

        await next();
    }
}
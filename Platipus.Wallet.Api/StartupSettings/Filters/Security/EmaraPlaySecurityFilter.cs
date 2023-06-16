using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

public sealed class EmaraPlaySecurityFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync( //TODO fix spacing
        ActionExecutingContext context,
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IEmaraPlayBaseRequest>()
            .Single();

        var httpContext = context.HttpContext;

        //TODO check if can be resolved from constructor, using service locator is not good
        var dbContext = httpContext.RequestServices.GetRequiredService<WalletDbContext>();

        var session = await dbContext
            .Set<Session>()
            .Where(s => s.Id == request.Token)
            .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    // CasinoProvider = s.User.Casino.Params.EmaraPlayProvider,//TODO
                })
            .FirstOrDefaultAsync();

        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = EmaraPlayResultFactory
                .Failure<EmaraPlayErrorResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed)
                .ToActionResult();
            return;
        }

        string authHeader = context.HttpContext.Request.Headers.Authorization!;

        authHeader = authHeader.Replace("Bearer ", "");

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();

        var result = EmaraPlaySecurityHash.IsValid(authHeader, requestBytesToValidate, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = EmaraPlayResultFactory.Failure(EmaraPlayErrorCode.InvalidHashCode).ToActionResult();
            return;
        }

        await next();
    }
}
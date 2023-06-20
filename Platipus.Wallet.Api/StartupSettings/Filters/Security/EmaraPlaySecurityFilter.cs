using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Extensions;
using Platipus.Wallet.Api.Extensions.SecuritySign;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.StartupSettings.Filters.Security;

public sealed class EmaraPlaySecurityFilter : IAsyncActionFilter
{
    private readonly WalletDbContext _dbContext;

    public EmaraPlaySecurityFilter(WalletDbContext walletDbContext)
    {
        _dbContext = walletDbContext;
    }

    public async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate next)
    {
        var request = context.ActionArguments.Values
            .OfType<IEmaraPlayBaseRequest>()
            .Single();
        

        var session = await _dbContext
            .Set<Session>()
            .Where(s => s.Id == request.Token)
            .Select(
                s => new
                {
                    s.ExpirationDate,
                    CasinoSignatureKey = s.User.Casino.SignatureKey,
                    CasinoProvider = s.User.Casino.Params.EmaraPlayProvider
                })
            .FirstOrDefaultAsync();
        
        if (session is null || session.ExpirationDate < DateTime.UtcNow)
        {
            context.Result = EmaraPlayResultFactory
                .Failure<EmaraPlayErrorResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed)
                .ToActionResult();
            return;
        }

        const int emaraplayProviderId = (int)CasinoProvider.EmaraPlay; 
        if (!session.CasinoProvider.Equals(emaraplayProviderId.ToString()))
        {
            context.Result = EmaraPlayResultFactory
                .Failure<EmaraPlayErrorResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed)
                .ToActionResult();
            return;
        }

        string authHeader = context.HttpContext.Request.Headers.Authorization!;
        if (string.IsNullOrWhiteSpace(authHeader))
        {
            context.Result = EmaraPlayResultFactory.Failure<EmaraPlayErrorResponse>(
                EmaraPlayErrorCode.PlayerAuthenticationFailed).ToActionResult();
            return;
        }
        
        authHeader = authHeader.Replace("Bearer ", "");

        var requestBytesToValidate = context.HttpContext.GetRequestBodyBytesItem();

        var result = EmaraPlaySecurityHash.IsValid(authHeader, requestBytesToValidate, session.CasinoSignatureKey);

        if (!result)
        {
            context.Result = EmaraPlayResultFactory.Failure<EmaraPlayErrorResponse>(
                EmaraPlayErrorCode.InvalidHashCode).ToActionResult();
            return;
        }

        await next();
    }
}
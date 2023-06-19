using Microsoft.AspNetCore.Mvc.Filters;
using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform;
using Platipus.Wallet.Api.Application.Requests.Wallets.AtlasPlatform.Base;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

public sealed class AtlasPlatformMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public AtlasPlatformMockedErrorActionFilter(
        ILogger<AtlasPlatformMockedErrorActionFilter> logger) 
        : base(logger) { }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IAtlasPlatformRequest)baseRequest;

        MockedErrorMethod? walletMethod = request switch
        {
            AtlasPlatformGetClientBalanceRequest => MockedErrorMethod.Balance,
            AtlasPlatformBetRequest => MockedErrorMethod.Bet,
            AtlasPlatformWinRequest => MockedErrorMethod.Win,
            AtlasPlatformRefundRequest => MockedErrorMethod.Rollback,
            _ => null
        };

        if (walletMethod is not null) 
            return new MockedErrorIdentifiers(walletMethod!.Value, request.Token!, true);
        
        Logger.LogInformation("Unexpected request type encountered: {RequestType}", request.GetType().Name);
        return null;

    }
}
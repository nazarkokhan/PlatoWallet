using Microsoft.AspNetCore.Mvc.Filters;
using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

public sealed class EmaraPlayMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public EmaraPlayMockedErrorActionFilter(ILogger<EmaraPlayMockedErrorActionFilter> logger) 
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest, ActionExecutedContext actionExecutedContext)
    {
        var request = (IEmaraPlayBaseRequest)baseRequest;


        var walletMethod = request switch
        {
            EmaraPlayBalanceRequest => MockedErrorMethod.Balance,
            EmaraPlayBetRequest => MockedErrorMethod.Bet,
            EmaraPlayResultRequest => MockedErrorMethod.Win,
            EmaraPlayRefundRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException(nameof(request))
        };
        // TODO walletMethod is never null
        // if (walletMethod is null)
        // {
        //     Logger.LogInformation("Unexpected request type encountered: {RequestType}", request.GetType().Name);
        //     return null;
        // }

        return new MockedErrorIdentifiers(walletMethod, request.Token!, true);
    }
}
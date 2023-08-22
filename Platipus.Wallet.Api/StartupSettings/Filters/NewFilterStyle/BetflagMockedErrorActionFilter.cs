namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Betflag;
using Application.Requests.Wallets.Betflag.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public class BetflagMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public BetflagMockedErrorActionFilter(
        ILogger<BetflagMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IBetflagRequest)baseRequest;

        var walletMethod = request switch
        {
            BetflagBalanceRequest => MockedErrorMethod.Balance,
            BetflagBetRequest => MockedErrorMethod.Bet,
            BetflagWinRequest => MockedErrorMethod.Win,
            BetflagCancelRequest => MockedErrorMethod.Rollback,
            BetflagAuthenticateRequest => MockedErrorMethod.Authenticate,
            _ => throw new ArgumentOutOfRangeException()
        };

        return new MockedErrorIdentifiers(walletMethod, request.Key, true);
    }
}
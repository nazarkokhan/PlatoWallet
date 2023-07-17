namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Uranus;
using Application.Requests.Wallets.Uranus.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class UranusMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public UranusMockedErrorActionFilter(ILogger<UranusMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IUranusRequest)baseRequest;

        var walletMethod = request switch
        {
            UranusBalanceRequest => MockedErrorMethod.Balance,
            UranusWithdrawRequest => MockedErrorMethod.Bet,
            UranusDepositRequest => MockedErrorMethod.Win,
            UranusPromoWinRequest => MockedErrorMethod.Award,
            UranusRollbackRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.SessionToken!, true);
    }
}
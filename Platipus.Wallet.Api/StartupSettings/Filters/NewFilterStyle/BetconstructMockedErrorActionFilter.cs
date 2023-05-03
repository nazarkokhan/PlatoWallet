namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.BetConstruct;
using Application.Requests.Wallets.BetConstruct.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public class BetconstructMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public BetconstructMockedErrorActionFilter(
        ILogger<BetconstructMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IBetconstructBoxRequest<IBetconstructRequest>)baseRequest;
        var requestData = request.Data;

        var walletMethod = requestData switch
        {
            BetconstructGetPlayerInfoRequest => MockedErrorMethod.Balance,
            BetConstructWithdrawRequest => MockedErrorMethod.Bet,
            BetconstructDepositRequest => MockedErrorMethod.Win,
            BetConstructRollbackTransactionRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException()
        };

        return new MockedErrorIdentifiers(walletMethod, requestData.Token, true);
    }
}
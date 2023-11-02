using Microsoft.AspNetCore.Mvc.Filters;
using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

public sealed class SweepiumMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public SweepiumMockedErrorActionFilter(
        ILogger<SweepiumMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (ISweepiumBoxRequest<ISweepiumRequest>)baseRequest;
        var requestData = request.Data;

        var walletMethod = requestData switch
        {
            SweepiumStartUpdateBalanceRequest => MockedErrorMethod.Balance,
            SweepiumBetRequest => MockedErrorMethod.Bet,
            SweepiumWinRequest => MockedErrorMethod.Win,
            SweepiumRollbackRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException()
        };

        return new MockedErrorIdentifiers(walletMethod, requestData.Token, true);
    }
}
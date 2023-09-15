namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Vegangster;
using Application.Requests.Wallets.Vegangster.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class VegangsterMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public VegangsterMockedErrorActionFilter(ILogger<VegangsterMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IVegangsterBaseRequest)baseRequest;

        var walletMethod = request switch
        {
            VegangsterPlayerBalanceRequest => MockedErrorMethod.Balance,
            VegangsterBetRequest => MockedErrorMethod.Bet,
            VegangsterWinRequest => MockedErrorMethod.Win,
            VegangsterRollbackRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.Token, true);
    }
}
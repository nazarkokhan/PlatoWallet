namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Nemesis;
using Application.Requests.Wallets.Nemesis.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class NemesisMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public NemesisMockedErrorActionFilter(ILogger<NemesisMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (INemesisRequest)baseRequest;

        var walletMethod = request switch
        {
            NemesisBalanceRequest => MockedErrorMethod.Balance,
            NemesisBetRequest => MockedErrorMethod.Bet,
            NemesisWinRequest => MockedErrorMethod.Win,
            NemesisCancelTransactionRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.SessionToken, true);
    }
}
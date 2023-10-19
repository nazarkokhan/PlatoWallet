namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Synot;
using Application.Requests.Wallets.Synot.Base;
using Constants.Synot;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class SynotMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public SynotMockedErrorActionFilter(ILogger<SynotMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (ISynotBaseRequest)baseRequest;

        if (!actionExecutedContext.HttpContext.Request.Headers.TryGetValue(SynotConstants.XEasToken, out var token))
        {
            return new MockedErrorIdentifiers();
        }

        var walletMethod = request switch
        {
            SynotGetBalanceRequest => MockedErrorMethod.Balance,
            SynotBetRequest => MockedErrorMethod.Bet,
            SynotWinRequest => MockedErrorMethod.Win,
            SynotRefundRequest => MockedErrorMethod.Rollback,
            SynotSessionRequest => MockedErrorMethod.Authenticate,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, token!, true);
    }
}
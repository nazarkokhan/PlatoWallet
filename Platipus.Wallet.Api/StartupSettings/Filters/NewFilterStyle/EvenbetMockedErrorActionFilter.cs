namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Evenbet.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class EvenbetMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public EvenbetMockedErrorActionFilter(ILogger<EvenbetMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IEvenbetRequest)baseRequest;

        // var walletMethod = request switch
        // {
        //     EvenbetGetBalanceRequest => MockedErrorMethod.Balance,
        //     _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        // };

        return new MockedErrorIdentifiers(MockedErrorMethod.Balance, request.Token, true);
    }
}
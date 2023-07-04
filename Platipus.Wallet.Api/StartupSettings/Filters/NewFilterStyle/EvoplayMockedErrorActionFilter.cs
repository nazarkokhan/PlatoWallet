namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Evoplay;
using Application.Requests.Wallets.Evoplay.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class EvoplayMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public EvoplayMockedErrorActionFilter(
        ILogger<EvoplayMockedErrorActionFilter> logger) : base(logger) { }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IEvoplayRequest)baseRequest;

        var walletMethod = request switch
        {
            EvoplayBalanceRequest => MockedErrorMethod.Balance,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.SessionToken!, true);

    }
}
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

        var walletMethod = MockedErrorMethod.Award;

        return new MockedErrorIdentifiers(walletMethod, request.Token, true);
    }
}
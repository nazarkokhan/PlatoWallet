using Microsoft.AspNetCore.Mvc.Filters;
using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle.Other;
using Platipus.Wallet.Domain.Entities.Enums;

namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Wallets.Atlas;
using Application.Requests.Wallets.Atlas.Base;

public sealed class AtlasMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public AtlasMockedErrorActionFilter(
        ILogger<AtlasMockedErrorActionFilter> logger) 
        : base(logger) { }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IAtlasRequest)baseRequest;

        var walletMethod = request switch
        {
            AtlasGetClientBalanceRequest => MockedErrorMethod.Balance,
            AtlasBetRequest => MockedErrorMethod.Bet,
            AtlasWinRequest => MockedErrorMethod.Win,
            AtlasRefundRequest => MockedErrorMethod.Rollback,
            AtlasAuthorizationRequest => MockedErrorMethod.Authenticate,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.Token!, true);

    }
}
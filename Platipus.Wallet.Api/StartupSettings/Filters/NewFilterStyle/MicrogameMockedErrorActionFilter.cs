namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Microgame;
using Application.Requests.Wallets.Microgame.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class MicrogameMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public MicrogameMockedErrorActionFilter(ILogger<MicrogameMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        if (baseRequest is MicrogameAuthenticateRequest authenticateRequest)
        {
            return new MockedErrorIdentifiers(MockedErrorMethod.Authenticate, authenticateRequest.SessionId, true);
        }

        var request = (IMicrogameMoneyOperationsRequest)baseRequest;

        var walletMethod = request switch
        {
            MicrogameGetBalanceRequest => MockedErrorMethod.Balance,
            MicrogameReserveRequest => MockedErrorMethod.Bet,
            MicrogameReleaseRequest => MockedErrorMethod.Win,
            MicrogameCancelReserveRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.ExternalId, false);
    }
}
namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Parimatch;
using Application.Requests.Wallets.Parimatch.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class ParimatchMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public ParimatchMockedErrorActionFilter(ILogger<ParimatchMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IParimatchRequest)baseRequest;

        var walletMethod = request switch
        {
            ParimatchPlayerInfoRequest => MockedErrorMethod.Balance,
            ParimatchBetRequest => MockedErrorMethod.Bet,
            ParimatchWinRequest => MockedErrorMethod.Win,
            ParimatchCancelRequest => MockedErrorMethod.Rollback,
            ParimatchPromoWinRequest => MockedErrorMethod.Award,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        if (request is IParimatchSessionRequest parimatchSessionRequest)
        {
            return new MockedErrorIdentifiers(walletMethod, parimatchSessionRequest.SessionToken, true);
        }

        var parimatchPlayerIdRequest = (IParimatchPlayerIdRequest)request;
        return new MockedErrorIdentifiers(walletMethod, parimatchPlayerIdRequest.PlayerId, false);
    }
}
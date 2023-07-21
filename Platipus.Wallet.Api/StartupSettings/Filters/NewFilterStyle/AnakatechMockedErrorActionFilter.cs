namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Anakatech;
using Application.Requests.Wallets.Anakatech.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public sealed class AnakatechMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public AnakatechMockedErrorActionFilter(ILogger<AnakatechMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (IAnakatechBaseRequest)baseRequest;

        var walletMethod = request switch
        {
            AnakatechGetPlayerBalanceRequest => MockedErrorMethod.Balance,
            AnakatechCreditRequest => MockedErrorMethod.Win,
            AnakatechDebitRequest => MockedErrorMethod.Bet,
            AnakatechRollbackRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException(nameof(baseRequest), "There is no such method in controller.")
        };

        return new MockedErrorIdentifiers(walletMethod, request.SessionId, true);
    }
}
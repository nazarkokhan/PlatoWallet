namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Softswiss;
using Application.Requests.Wallets.Softswiss.Base;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public class SoftswissMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    public SoftswissMockedErrorActionFilter(ILogger<SoftswissMockedErrorActionFilter> logger)
        : base(logger)
    {
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (ISoftswissBaseRequest)baseRequest;

        var walletMethod = request switch
        {
            SoftswissPlayRequest playRequest => playRequest.Actions?.SingleOrDefault()?.Action switch
            {
                null => MockedErrorMethod.Balance,
                "bet" => MockedErrorMethod.Bet,
                "win" => MockedErrorMethod.Win,
                _ => throw new ArgumentOutOfRangeException()
            },
            SoftswissRollbackRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException()
        };

        return new MockedErrorIdentifiers(walletMethod, request.SessionId, true);
    }
}
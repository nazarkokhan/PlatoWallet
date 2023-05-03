namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities.Enums;
using Microsoft.AspNetCore.Mvc.Filters;
using Other;

public class OpenboxMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    private readonly ILogger<OpenboxMockedErrorActionFilter> _logger;

    public OpenboxMockedErrorActionFilter(
        ILogger<OpenboxMockedErrorActionFilter> logger)
        : base(logger)
    {
        _logger = logger;
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(
        IBaseWalletRequest baseRequest,
        ActionExecutedContext actionExecutedContext)
    {
        var request = (OpenboxSingleRequest)baseRequest;

        if (!actionExecutedContext.HttpContext.Items.TryGetValue(
                HttpContextItems.OpenboxPayloadRequestObj,
                out var payloadRequestObj)
         || payloadRequestObj is not IOpenboxBaseRequest openboxPayloadObj)
        {
            _logger.LogCritical("Failed mocking openbox request. {OpenboxRequest}", request);
            return null;
        }

        MockedErrorMethod? walletMethod = request.Method switch
        {
            OpenboxHelpers.GetPlayerBalance => MockedErrorMethod.Balance,
            OpenboxHelpers.MoneyTransactions => (openboxPayloadObj as OpenboxMoneyTransactionRequest)?.OrderType switch
            {
                3 => MockedErrorMethod.Bet,
                4 => MockedErrorMethod.Win,
                _ => null
            },
            OpenboxHelpers.CancelTransaction => MockedErrorMethod.Rollback,
            _ => null
        };

        if (walletMethod is null)
            return null;

        return new MockedErrorIdentifiers(walletMethod.Value, openboxPayloadObj.Token, true);
    }
}
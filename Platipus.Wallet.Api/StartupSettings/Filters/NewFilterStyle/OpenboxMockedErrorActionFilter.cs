namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Openbox;
using Application.Requests.Wallets.Openbox.Base;
using Platipus.Wallet.Api.Extensions;
using Domain.Entities.Enums;
using Other;

public class OpenboxMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    private readonly ILogger<OpenboxMockedErrorActionFilter> _logger;
    private readonly HttpContext _httpContext;

    public OpenboxMockedErrorActionFilter(
        ILogger<OpenboxMockedErrorActionFilter> logger,
        IHttpContextAccessor httpContextAccessor)
        : base(logger)
    {
        _logger = logger;
        _httpContext = httpContextAccessor.HttpContext!;
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(IBaseWalletRequest baseRequest)
    {
        var request = (OpenboxSingleRequest)baseRequest;

        if (!_httpContext.Items.TryGetValue(HttpContextItems.OpenboxPayloadRequestObj, out var payloadRequestObj)
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
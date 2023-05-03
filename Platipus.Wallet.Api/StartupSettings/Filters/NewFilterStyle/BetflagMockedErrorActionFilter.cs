namespace Platipus.Wallet.Api.StartupSettings.Filters.NewFilterStyle;

using Application.Requests.Base;
using Application.Requests.Wallets.Betflag;
using Application.Requests.Wallets.Betflag.Base;
using Domain.Entities.Enums;
using Other;

public class BetflagMockedErrorActionFilter : AbstractMockedErrorActionFilter
{
    private readonly ILogger<BetflagMockedErrorActionFilter> _logger;

    public BetflagMockedErrorActionFilter(
        ILogger<BetflagMockedErrorActionFilter> logger)
        : base(logger)
    {
        _logger = logger;
    }

    protected override MockedErrorIdentifiers? GetMockedErrorIdentifiers(IBaseWalletRequest baseRequest)
    {
        var request = (IBetflagRequest)baseRequest;

        var walletMethod = request switch
        {
            BetflagBalanceRequest => MockedErrorMethod.Balance,
            BetflagBetRequest => MockedErrorMethod.Bet,
            BetflagWinRequest => MockedErrorMethod.Win,
            BetflagCancelRequest => MockedErrorMethod.Rollback,
            _ => throw new ArgumentOutOfRangeException()
        };

        return new MockedErrorIdentifiers(walletMethod, request.Key, true);
    }
}
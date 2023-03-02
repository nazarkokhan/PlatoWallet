namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Extensions;
using Infrastructure.Persistence;
using Results.Betflag.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.Betflag.BetflagResultFactory;

public record BetflagWinRequest(
    string Key,
    string TransactionId,
    string RoundId,
    bool RoundCompleted,
    double Win,
    double WinJp,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBetWinCancelRequest
{
    public class Handler : IRequestHandler<BetflagWinRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.WinAsync(
                request.Key,
                request.RoundId,
                request.TransactionId,
                (decimal)request.Win,
                request.RoundCompleted,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetflagResult<BetflagBetWinCancelResponse>();

            var response = walletResult.Data.Map(
                d => new BetflagBetWinCancelResponse(
                    (double)d.Balance,
                    d.Currency));

            return Success(response);
        }
    }
}
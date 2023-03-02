namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Extensions;
using Infrastructure.Persistence;
using Results.Betflag.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.Betflag.BetflagResultFactory;

public record BetflagCancelRequest(
    string Key,
    string TransactionId,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBetWinCancelRequest
{
    public class Handler : IRequestHandler<BetflagCancelRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagCancelRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.Key,
                request.TransactionId,
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
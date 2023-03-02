namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Extensions;
using Results.Betflag.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using static Results.Betflag.BetflagResultFactory;

public record BetflagBetRequest(
    string Key,
    string TransactionId,
    string RoundId,
    double Bet,
    bool FreeSpin,
    double QuotaJP,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBetWinCancelResponse>>, IBetflagBetWinCancelRequest
{
    public class Handler : IRequestHandler<BetflagBetRequest, IBetflagResult<BetflagBetWinCancelResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetflagResult<BetflagBetWinCancelResponse>> Handle(
            BetflagBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.Key,
                request.RoundId,
                request.TransactionId,
                (decimal)request.Bet,
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
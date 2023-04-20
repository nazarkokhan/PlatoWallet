namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Base;
using Base.Response;
using Results.BetConstruct;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record BetConstructWithdrawRequest(
        string Token,
        string TransactionId,
        string RoundId,
        string GameId,
        string CurrencyId,
        decimal BetAmount,
        string BetInfo)
    : IBetconstructRequest, IRequest<IBetconstructResult<BetconstructPlayResponse>>
{
    public class Handler : IRequestHandler<BetConstructWithdrawRequest, IBetconstructResult<BetconstructPlayResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IBetconstructResult<BetconstructPlayResponse>> Handle(
            BetConstructWithdrawRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                request.BetAmount,
                request.CurrencyId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToBetConstructResult<BetconstructPlayResponse>();
            var data = walletResult.Data;

            var response = new BetconstructPlayResponse(
                data.Transaction.Id,
                data.Balance);

            return BetconstructResultFactory.Success(response);
        }
    }
}
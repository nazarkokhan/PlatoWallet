namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record EverymatrixBetRequest(
        string Token,
        decimal Amount,
        string Currency,
        int GameId,
        string RoundId,
        string ExternalId,
        string Hash,
        EverymatrixJackpotContribution? JackpotContribution)
    : IEveryMatrixRequest, IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>
{
    public class Handler : IRequestHandler<EverymatrixBetRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.Token,
                request.RoundId,
                request.ExternalId,
                request.Amount,
                request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();
            var data = walletResult.Data;

            var response = new EverymatrixBalanceResponse(data.Balance, data.Currency);

            return EverymatrixResultFactory.Success(response);
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Results.Everymatrix;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record EverymatrixWinRequest(
    string Token,
    decimal Amount,
    string Currency,
    string? BonusId,
    int GameId,
    string RoundId,
    string ExternalId,
    string BetExternalId,
    string Hash,
    bool? RoundEnd,
    EverymatrixJackpotPayout? JackpotPayout) : IRequest<IEverymatrixResult<EverymatrixBalanceResponse>>, IEveryMatrixRequest
{
    public class Handler : IRequestHandler<EverymatrixWinRequest, IEverymatrixResult<EverymatrixBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixWinRequest request,
            CancellationToken cancellationToken)
        {
            EverymatrixBalanceResponse response;

            if (request.BonusId is not null)
            {
                var walletResult = await _wallet.AwardAsync(
                    request.Token,
                    request.RoundId,
                    request.ExternalId,
                    request.Amount,
                    request.BonusId,
                    request.Currency,
                    cancellationToken: cancellationToken);

                if (walletResult.IsFailure)
                    return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();
                var data = walletResult.Data;

                response = new EverymatrixBalanceResponse(data.Balance, data.Currency);
            }
            else
            {
                var walletResult = await _wallet.WinAsync(
                    request.Token,
                    request.RoundId,
                    request.ExternalId,
                    request.Amount,
                    request.RoundEnd ?? false,
                    request.Currency,
                    cancellationToken: cancellationToken);

                if (walletResult.IsFailure)
                    return walletResult.ToEverymatrixResult<EverymatrixBalanceResponse>();
                var data = walletResult.Data;

                response = new EverymatrixBalanceResponse(data.Balance, data.Currency);
            }

            return EverymatrixResultFactory.Success(response);
        }
    }
}
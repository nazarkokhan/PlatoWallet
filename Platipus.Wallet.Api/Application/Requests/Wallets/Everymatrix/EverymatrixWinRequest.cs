namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using Base;
using Base.Response;
using Infrastructure.Persistence;
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
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixBalanceResponse>> Handle(
            EverymatrixWinRequest request,
            CancellationToken cancellationToken)
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

            var response = new EverymatrixBalanceResponse(data.Balance, data.Currency);

            return EverymatrixResultFactory.Success(response);
        }
    }
}
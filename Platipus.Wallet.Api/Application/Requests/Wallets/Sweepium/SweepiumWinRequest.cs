using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumWinRequest(
        string Token,
        string TransactionId,
        string RoundId,
        string GameId,
        string CurrencyId,
        decimal WinAmount)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumSuccessResponse>>
{
    public sealed class Handler
        : IRequestHandler<SweepiumWinRequest, ISweepiumResult<SweepiumSuccessResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<ISweepiumResult<SweepiumSuccessResponse>> Handle(
            SweepiumWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token,
                request.RoundId,
                request.TransactionId,
                request.WinAmount,
                currency: request.CurrencyId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSweepiumResult<SweepiumSuccessResponse>();

            var data = walletResult.Data;

            var response = new SweepiumSuccessResponse(
                data.Transaction.Id,
                data.Balance);

            return SweepiumResultFactory.Success(response);
        }
    }
}
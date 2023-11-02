using System.ComponentModel;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Responses.Sweepium.Base;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumRollbackRequest(
        string Token,
        string TransactionId,
        string RoundId,
        string GameId)
    : ISweepiumRequest, IRequest<ISweepiumResult<SweepiumSuccessResponse>>
{
    public sealed class Handler
        : IRequestHandler<SweepiumRollbackRequest, ISweepiumResult<SweepiumSuccessResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => 
            _walletService = walletService;

        public async Task<ISweepiumResult<SweepiumSuccessResponse>> Handle(
            SweepiumRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.Token,
                request.TransactionId,
                request.RoundId,
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
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium.Base;
using Platipus.Wallet.Api.Application.Responses.Sweepium;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Sweepium;

public sealed record SweepiumRollbackRequest(
        string Token,
        [property: JsonPropertyName("transactionId")] [property: BindProperty(Name = "transactionId")] string TransactionId,
        [property: JsonPropertyName("roundId")] [property: BindProperty(Name = "roundId")] string RoundId,
        [property: JsonPropertyName("gameId")] [property: BindProperty(Name = "gameId")] string GameId)
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
                (int)MoneyHelper.ConvertToCents(data.Balance));

            return SweepiumResultFactory.Success(response);
        }
    }
}
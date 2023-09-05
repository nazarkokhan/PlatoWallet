namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot;

using System.Text.Json.Serialization;
using Base;
using Helpers.Common;
using Models;
using Responses.Synot;
using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Services.Wallet;

public sealed record SynotBetRequest(
        long Id,
        long RoundId,
        long Amount,
        string? Token,
        [property: JsonPropertyName("transactionDetail")] SynotTransactionDetail? TransactionDetail,
        [property: JsonPropertyName("freeGameOffer")] string? FreeGameOffer)
    : ISynotOperationsRequest, IRequest<ISynotResult<SynotOperationsResponse>>
{
    public sealed class Handler : IRequestHandler<SynotBetRequest, ISynotResult<SynotOperationsResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<ISynotResult<SynotOperationsResponse>> Handle(
            SynotBetRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.Token!,
                roundId: request.RoundId.ToString(),
                transactionId: request.Id.ToString(),
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToSynotFailureResult<SynotOperationsResponse>();

            var data = walletResult.Data;

            var response = new SynotOperationsResponse(
                long.Parse(data.Transaction.Id),
                MoneyHelper.ConvertToCents(data.Balance));

            return walletResult.ToSynotResult(response);
        }
    }
}
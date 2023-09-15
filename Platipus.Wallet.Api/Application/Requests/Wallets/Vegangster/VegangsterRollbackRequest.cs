namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using System.Text.Json.Serialization;
using Base;
using Helpers;
using Responses.Vegangster;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Services.Wallet;

public sealed record VegangsterRollbackRequest(
        string Token,
        string PlayerId,
        string GameCode,
        string TransactionId,
        string RoundId,
        [property: JsonPropertyName("reference_transaction_id")] string ReferenceTransactionId)
    : IVegangsterTransactionRequest, IRequest<IVegangsterResult<VegangsterTransactionResponse>>
{
    public sealed class Handler
        : IRequestHandler<VegangsterRollbackRequest, IVegangsterResult<VegangsterTransactionResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IVegangsterResult<VegangsterTransactionResponse>> Handle(
            VegangsterRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.Token,
                roundId: request.RoundId,
                transactionId: request.ReferenceTransactionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToVegangsterFailureResult<VegangsterTransactionResponse>();

            var data = walletResult.Data;

            var response = new VegangsterTransactionResponse(
                VegangsterResponseStatus.OK.ToString(),
                data.Currency,
                (int)MoneyHelper.ConvertToCents(data.Balance),
                data.Transaction.Id);

            return walletResult.ToVegangsterResult(response);
        }
    }
}
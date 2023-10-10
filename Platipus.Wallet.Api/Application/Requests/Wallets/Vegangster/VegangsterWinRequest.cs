namespace Platipus.Wallet.Api.Application.Requests.Wallets.Vegangster;

using System.Text.Json.Serialization;
using Base;
using Helpers;
using Responses.Vegangster;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Services.Wallet;

public sealed record VegangsterWinRequest(
        string Token,
        string PlayerId,
        string GameCode,
        string TransactionId,
        string RoundId,
        string Currency,
        long Amount,
        [property: JsonPropertyName("freegames_reference")] string? FreeGamesReference = null)
    : IVegangsterTransactionRequest, IRequest<IVegangsterResult<VegangsterTransactionResponse>>
{
    public sealed class Handler : IRequestHandler<VegangsterWinRequest, IVegangsterResult<VegangsterTransactionResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<IVegangsterResult<VegangsterTransactionResponse>> Handle(
            VegangsterWinRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.Token,
                roundId: request.RoundId,
                transactionId: request.TransactionId,
                amount: MoneyHelper.ConvertFromCents(request.Amount),
                currency: request.Currency,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToVegangsterFailureResult<VegangsterTransactionResponse>();

            var data = walletResult.Data;

            var response = new VegangsterTransactionResponse(
                VegangsterResponseStatus.OK.ToString(),
                data.Currency,
                MoneyHelper.ConvertToCents(data.Balance),
                data.Transaction.Id);

            return walletResult.ToVegangsterResult(response);
        }
    }
}
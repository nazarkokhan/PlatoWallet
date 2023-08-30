namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis;

using System.Text.Json.Serialization;
using Base;
using JetBrains.Annotations;
using Responses;
using Results.Nemesis;
using Results.Nemesis.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public sealed record NemesisCancelTransactionRequest(
        [property: JsonPropertyName("session_token")] string SessionToken,
        [property: JsonPropertyName("player_id")] string PlayerId,
        [property: JsonPropertyName("game_id")] string GameId,
        [property: JsonPropertyName("round_id")] string RoundId,
        [property: JsonPropertyName("reference_transaction_id")] string ReferenceTransactionId,
        [property: JsonPropertyName("amount")] decimal Amount,
        [property: JsonPropertyName("currency")] string Currency)
    : INemesisRequest, IRequest<INemesisResult<NemesisCancelTransactionResponse>>
{
    public sealed class
        Handler : IRequestHandler<NemesisCancelTransactionRequest, INemesisResult<NemesisCancelTransactionResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public async Task<INemesisResult<NemesisCancelTransactionResponse>> Handle(
            NemesisCancelTransactionRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.SessionToken,
                request.ReferenceTransactionId,
                request.RoundId,
                amount: NemesisMoneyHelper.ToBalance(request.Amount, request.Currency),
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToNemesisFailureResult<NemesisCancelTransactionResponse>();
            var data = walletResult.Data;

            var response = new NemesisCancelTransactionResponse(
                data.Transaction.Id,
                data.Transaction.InternalId,
                NemesisMoneyHelper.FromBalance(data.Balance, data.Currency, out var multiplier),
                data.Currency,
                multiplier);

            return NemesisResultFactory.Success(response);
        }
    }
}
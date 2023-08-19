namespace Platipus.Wallet.Api.Application.Requests.Wallets.Nemesis;

using Base;
using Infrastructure.Persistence;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Responses;
using Results.Nemesis;
using Results.Nemesis.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

[PublicAPI]
public sealed record NemesisCancelTransactionRequest(
        [property: JsonProperty("session_token")] string SessionToken,
        [property: JsonProperty("player_id")] string PlayerId,
        [property: JsonProperty("game_id")] bool GameId,
        [property: JsonProperty("round_id")] string RoundId,
        [property: JsonProperty("reference_transaction_id")] string ReferenceTransactionId,
        [property: JsonProperty("amount")] decimal Amount,
        [property: JsonProperty("currency")] string Currency)
    : INemesisRequest, IRequest<INemesisResult<NemesisBetWinRollbackResponse>>
{
    public sealed class
        Handler : IRequestHandler<NemesisCancelTransactionRequest, INemesisResult<NemesisBetWinRollbackResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly WalletDbContext _context;

        public Handler(IWalletService walletService, WalletDbContext context)
        {
            _walletService = walletService;
            _context = context;
        }

        public async Task<INemesisResult<NemesisBetWinRollbackResponse>> Handle(
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
                return walletResult.ToNemesisFailureResult<NemesisBetWinRollbackResponse>();
            var data = walletResult.Data;

            var response = new NemesisBetWinRollbackResponse(
                data.Transaction.InternalId,
                data.Transaction.Id,
                NemesisMoneyHelper.FromBalance(data.Balance, data.Currency),
                data.Currency);

            return NemesisResultFactory.Success(response);
        }
    }
}
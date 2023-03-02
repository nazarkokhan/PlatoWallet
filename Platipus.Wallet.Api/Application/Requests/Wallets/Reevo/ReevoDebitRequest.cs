namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo;

using Base;
using Results.Reevo;
using Results.Reevo.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record ReevoDebitRequest(
        string CallerId,
        string CallerPassword,
        string Action,
        int? RemoteId,
        string Username,
        string SessionId,
        double Amount,
        string GameIdHash,
        string TransactionId,
        string RoundId,
        int GameplayFinal,
        int IsFreeRoundBet,
        string? FreeRoundId,
        double? Fee,
        double? JackpotContributionInAmount,
        string GameSessionId,
        string Key)
    : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoDebitRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoDebitRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.BetAsync(
                request.SessionId,
                request.RoundId,
                request.TransactionId,
                (decimal)request.Amount,
                roundFinished: request.GameplayFinal is 1,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToReevoResult<ReevoSuccessResponse>();
            var data = walletResult.Data;

            var response = new ReevoSuccessResponse(data.Balance);

            return ReevoResultFactory.Success(response);
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo;

using Base;
using Results.Reevo;
using Results.Reevo.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record ReevoRollbackRequest(
    string CallerId,
    string CallerPassword,
    string Action,
    int RemoteId,
    string Username,
    string SessionId,
    double Amount,
    string? GameIdHash,
    string TransactionId,
    string RoundId,
    int GameplayFinal,
    string GameSessionId,
    string Key) : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoRollbackRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.RollbackAsync(
                request.SessionId,
                request.TransactionId,
                request.RoundId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToReevoResult<ReevoSuccessResponse>();
            var data = walletResult.Data;

            var response = new ReevoSuccessResponse(data.Balance);

            return ReevoResultFactory.Success(response);
        }
    }
}
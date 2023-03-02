namespace Platipus.Wallet.Api.Application.Requests.Wallets.Reevo;

using Base;
using Results.Reevo;
using Results.Reevo.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public record ReevoBalanceRequest(
    string CallerId,
    string CallerPassword,
    string Action,
    int RemoteId,
    string Username,
    string GameIdHash,
    string SessionId,
    string GameSessionId,
    string Key) : IRequest<IReevoResult<ReevoSuccessResponse>>, IReevoRequest
{
    public class Handler : IRequestHandler<ReevoBalanceRequest, IReevoResult<ReevoSuccessResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IReevoResult<ReevoSuccessResponse>> Handle(
            ReevoBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _wallet.GetBalanceAsync(
                request.SessionId,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToReevoResult<ReevoSuccessResponse>();
            var data = walletResult.Data;

            var response = new ReevoSuccessResponse(data.Balance);

            return ReevoResultFactory.Success(response);
        }
    }
}
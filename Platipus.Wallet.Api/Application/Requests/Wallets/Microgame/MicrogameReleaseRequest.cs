namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame;

using Base;
using Responses.Microgame;
using Results.Microgame.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record MicrogameReleaseRequest(
    string GameId,
    string ExternalId,
    string AccessToken,
    string TransactionId,
    string ExternalGameSessionId,
    decimal Real,
    string Currency,
    long RoundId,
    bool? State) : IRequest<IMicrogameResult<MicrogameReleaseResponse>>, IMicrogameMoneyOperationsRequest, IMicrogameCommonOperationsRequest
{
    public sealed class Handler : IRequestHandler<MicrogameReleaseRequest, IMicrogameResult<MicrogameReleaseResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => _walletService = walletService;

        public async Task<IMicrogameResult<MicrogameReleaseResponse>> Handle(
            MicrogameReleaseRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.WinAsync(
                request.ExternalId,
                roundId: request.RoundId.ToString(),
                transactionId: request.TransactionId,
                amount: request.Real,
                currency: request.Currency,
                searchByUsername: true,
                roundFinished: request.State ?? true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToMicrogameErrorResult<MicrogameReleaseResponse>();

            var data = walletResult.Data;

            var response = new MicrogameReleaseResponse(data.Transaction.InternalId, data.Currency, data.Balance);

            return walletResult.ToMicrogameResult(response);
        }
    }
}
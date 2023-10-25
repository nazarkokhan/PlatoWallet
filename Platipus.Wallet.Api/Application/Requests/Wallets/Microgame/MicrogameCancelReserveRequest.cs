namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame;

using Base;
using Responses.Microgame;
using Results.Microgame.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record MicrogameCancelReserveRequest(
    string GameId,
    string ExternalId,
    string AccessToken,
    string TransactionId,
    string ExternalGameSessionId,
    decimal Real,
    string Currency,
    long RoundId) : IRequest<IMicrogameResult<MicrogameCancelReserveResponse>>,
                    IMicrogameMoneyOperationsRequest,
                    IMicrogameCommonOperationsRequest
{
    public sealed class Handler : IRequestHandler<MicrogameCancelReserveRequest, IMicrogameResult<MicrogameCancelReserveResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => _walletService = walletService;

        public async Task<IMicrogameResult<MicrogameCancelReserveResponse>> Handle(
            MicrogameCancelReserveRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.RollbackAsync(
                request.ExternalId,
                roundId: request.RoundId.ToString(),
                transactionId: request.TransactionId,
                searchByUsername: true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToMicrogameErrorResult<MicrogameCancelReserveResponse>();

            var data = walletResult.Data;

            var response = new MicrogameCancelReserveResponse(data.Transaction.InternalId);

            return walletResult.ToMicrogameResult(response);
        }
    }
}
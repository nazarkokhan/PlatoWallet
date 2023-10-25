namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame;

using System.Text.Json.Serialization;
using Base;
using Responses.Microgame;
using Results.Microgame;
using Results.Microgame.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record MicrogameReserveRequest(
    string GameId,
    string ExternalId,
    string AccessToken,
    string TransactionId,
    string ExternalGameSessionId,
    decimal Real,
    string Currency,
    long RoundId,
    [property: JsonPropertyName("jackpotGain")] decimal JackpotGain) : IRequest<IMicrogameResult<MicrogameReserveResponse>>,
                                                                       IMicrogameBaseRequest,
                                                                       IMicrogameCommonOperationsRequest
{
    public sealed class Handler : IRequestHandler<MicrogameReserveRequest, IMicrogameResult<MicrogameReserveResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => _walletService = walletService;

        public async Task<IMicrogameResult<MicrogameReserveResponse>> Handle(
            MicrogameReserveRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.BetAsync(
                request.ExternalId,
                roundId: request.RoundId.ToString(),
                transactionId: request.TransactionId,
                amount: request.Real,
                currency: request.Currency,
                searchByUsername: true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToMicrogameErrorResult<MicrogameReserveResponse>();

            var data = walletResult.Data;

            var response = new MicrogameReserveResponse(
                data.Transaction.InternalId,
                data.Currency,
                data.Balance);

            return walletResult.ToMicrogameResult(response);
        }
    }
}
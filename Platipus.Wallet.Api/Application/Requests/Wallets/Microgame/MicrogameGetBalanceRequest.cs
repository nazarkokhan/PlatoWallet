namespace Platipus.Wallet.Api.Application.Requests.Wallets.Microgame;

using System.Text.Json.Serialization;
using Base;
using Responses.Microgame;
using Results.Microgame.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;

public sealed record MicrogameGetBalanceRequest(
        string AccessToken,
        [property: JsonPropertyName("currency")] string Currency,
        string GameId,
        string ExternalId)
    : IMicrogameBaseRequest, IRequest<IMicrogameResult<MicrogameGetBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<MicrogameGetBalanceRequest, IMicrogameResult<MicrogameGetBalanceResponse>>
    {
        private readonly IWalletService _walletService;

        public Handler(IWalletService walletService) => _walletService = walletService;

        public async Task<IMicrogameResult<MicrogameGetBalanceResponse>> Handle(
            MicrogameGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletResult = await _walletService.GetBalanceAsync(
                request.ExternalId,
                true,
                cancellationToken: cancellationToken);

            if (walletResult.IsFailure)
                return walletResult.ToMicrogameErrorResult<MicrogameGetBalanceResponse>();

            var data = walletResult.Data;

            var response = new MicrogameGetBalanceResponse(data.Currency, data.Balance);

            return walletResult.ToMicrogameResult(response);
        }
    }
}
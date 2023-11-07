namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi.Requests;

using System.Text.Json.Serialization;
using External;
using Helpers;
using Results.ISoftBet.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record SoftBetGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] SoftBetGetLaunchUrlGameApiRequest ApiRequest)
    : IRequest<ISoftBetResult<string>>
{
    public sealed class Handler : IRequestHandler<SoftBetGetLaunchUrlRequest, ISoftBetResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly ISoftBetGameApiClient _softBetGameApiClient;

        public Handler(IWalletService walletService, ISoftBetGameApiClient softBetGameApiClient)
        {
            _walletService = walletService;
            _softBetGameApiClient = softBetGameApiClient;
        }

        public async Task<ISoftBetResult<string>> Handle(
            SoftBetGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _softBetGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToSoftBetResult<string>();

            var launchUrl = clientResponse.Data.Data;

            return clientResponse.ToSoftBetResult(launchUrl);
        }
    }
}
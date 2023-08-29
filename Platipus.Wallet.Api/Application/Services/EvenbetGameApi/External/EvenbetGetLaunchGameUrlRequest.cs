namespace Platipus.Wallet.Api.Application.Services.EvenbetGameApi.External;

using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Wallet;
using Requests;

public sealed record EvenbetGetLaunchGameUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] EvenbetGetLaunchGameUrlGameApiRequest ApiRequest)
    : IRequest<IEvenbetResult<string>>
{
    public sealed class Handler : IRequestHandler<EvenbetGetLaunchGameUrlRequest, IEvenbetResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IEvenbetGameApiClient _evenbetGameApiClient;

        public Handler(
            IWalletService walletService,
            IEvenbetGameApiClient evenbetGameApiClient)
        {
            _walletService = walletService;
            _evenbetGameApiClient = evenbetGameApiClient;
        }

        public async Task<IEvenbetResult<string>> Handle(
            EvenbetGetLaunchGameUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _evenbetGameApiClient.GetGameLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToEvenbetFailureResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;
            var processedScript = Regex.Unescape(gameLaunchScript);

            return clientResponse.ToEvenbetResult(processedScript);
        }
    }
}
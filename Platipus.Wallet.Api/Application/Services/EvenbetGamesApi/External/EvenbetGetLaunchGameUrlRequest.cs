namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using Newtonsoft.Json;
using Requests;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record EvenbetGetLaunchGameUrlRequest(
        [property: JsonProperty("environment")] string Environment,
        [property: JsonProperty("apiRequest")] EvenbetGetLaunchGameUrlGameApiRequest ApiRequest)
    : IRequest<IEvenbetResult<EvenbetGetLaunchGameUrlResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetGetLaunchGameUrlRequest, IEvenbetResult<EvenbetGetLaunchGameUrlResponse>>
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

        public async Task<IEvenbetResult<EvenbetGetLaunchGameUrlResponse>> Handle(
            EvenbetGetLaunchGameUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            if (walletResponse.Data is null)
            {
                return walletResponse.ToEvenbetFailureResult<EvenbetGetLaunchGameUrlResponse>();
            }

            var clientResponse = await _evenbetGameApiClient.GetGameLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure || clientResponse.Data?.Data is null)
                return clientResponse.ToEvenbetFailureResult<EvenbetGetLaunchGameUrlResponse>();

            var gameUrl = clientResponse.Data.Data.Url;
            var response = new EvenbetGetLaunchGameUrlResponse(gameUrl);

            return clientResponse.ToEvenbetResult(response);
        }
    }
}
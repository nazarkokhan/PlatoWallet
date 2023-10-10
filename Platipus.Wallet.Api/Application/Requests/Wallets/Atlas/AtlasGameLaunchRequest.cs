namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using System.Text;
using System.Text.Json.Serialization;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;
using Services.AtlasGameApi;
using Services.AtlasGameApi.Requests;
using Services.Wallet;

public sealed record AtlasGameLaunchRequest(
    string Environment,
    [property: JsonPropertyName("apiRequest")] AtlasGameLaunchGameApiRequest ApiRequest) : IRequest<IAtlasResult<Uri>>
{
    public sealed class Handler : IRequestHandler<AtlasGameLaunchRequest, IAtlasResult<Uri>>
    {
        private readonly IWalletService _walletService;
        private readonly IAtlasGameApiClient _atlasGameApiClient;

        public Handler(
            IWalletService walletService,
            IAtlasGameApiClient atlasGameApiClient)
        {
            _walletService = walletService;
            _atlasGameApiClient = atlasGameApiClient;
        }

        public async Task<IAtlasResult<Uri>> Handle(
            AtlasGameLaunchRequest request,
            CancellationToken cancellationToken)
        {
            const string stringToEncode = "atlas:password123";
            var stringToEncodeAsBytes = Encoding.UTF8.GetBytes(stringToEncode);
            var token = Convert.ToBase64String(stringToEncodeAsBytes);

            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _atlasGameApiClient.LaunchGameAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                token,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToAtlasFailureResult<Uri>();

            var gameLaunchUrl = clientResponse.Data.Data.Url;

            return clientResponse.ToAtlasResult(gameLaunchUrl);
        }
    }
}
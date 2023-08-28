namespace Platipus.Wallet.Api.Application.Services.AnakatechGamesApi.External;

using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Requests;
using Results.Anakatech.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record AnakatechLaunchGameRequest(
    [property: JsonPropertyName("environment")] string Environment,
    [property: JsonPropertyName("apiRequest")] AnakatechLaunchGameApiRequest ApiRequest) : IRequest<IAnakatechResult<string>>
{
    public sealed class Handler : IRequestHandler<AnakatechLaunchGameRequest, IAnakatechResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IAnakatechGameApiClient _anakatechGameApiClient;

        public Handler(
            IWalletService walletService,
            IAnakatechGameApiClient anakatechGameApiClient)
        {
            _walletService = walletService;
            _anakatechGameApiClient = anakatechGameApiClient;
        }

        public async Task<IAnakatechResult<string>> Handle(
            AnakatechLaunchGameRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _anakatechGameApiClient.GetLaunchGameUrlAsBytesAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToAnakatechFailureResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;
            var processedScript = Regex.Unescape(gameLaunchScript);

            return clientResponse.ToAnakatechResult(processedScript);
        }
    }
}
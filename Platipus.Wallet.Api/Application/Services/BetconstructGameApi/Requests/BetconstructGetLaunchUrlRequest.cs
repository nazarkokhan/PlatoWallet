namespace Platipus.Wallet.Api.Application.Services.BetconstructGameApi.Requests;

using System.Text.Json.Serialization;
using External;
using Helpers;
using Results.BetConstruct.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record BetconstructGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] BetconstructGetLaunchScriptGameApiRequest ApiRequest)
    : IRequest<IBetconstructResult<string>>
{
    public sealed class Handler : IRequestHandler<BetconstructGetLaunchUrlRequest, IBetconstructResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IBetconstructGameApiClient _betconstructGameApiClient;

        public Handler(IWalletService walletService, IBetconstructGameApiClient betconstructGameApiClient)
        {
            _walletService = walletService;
            _betconstructGameApiClient = betconstructGameApiClient;
        }

        public async Task<IBetconstructResult<string>> Handle(
            BetconstructGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _betconstructGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToBetConstructResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;
            var launchUrl = ScriptHelper.ExtractUrlFromScript(gameLaunchScript, request.Environment);

            return clientResponse.ToBetConstructResult(launchUrl);
        }
    }
}
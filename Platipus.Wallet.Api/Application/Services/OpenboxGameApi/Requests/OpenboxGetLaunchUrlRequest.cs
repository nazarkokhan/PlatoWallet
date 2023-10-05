namespace Platipus.Wallet.Api.Application.Services.OpenboxGameApi.Requests;

using System.Text.Json.Serialization;
using External;
using Helpers;
using Results.ResultToResultMappers;
using Wallet;

public sealed record OpenboxGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] OpenboxGetLaunchScriptGameApiRequest ApiRequest)
    : IRequest<IOpenboxResult<string>>
{
    public sealed class Handler : IRequestHandler<OpenboxGetLaunchUrlRequest, IOpenboxResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IOpenboxGameApiClient _openboxGameApiClient;

        public Handler(
            IWalletService walletService,
            IOpenboxGameApiClient openboxGameApiClient)
        {
            _walletService = walletService;
            _openboxGameApiClient = openboxGameApiClient;
        }

        public async Task<IOpenboxResult<string>> Handle(
            OpenboxGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _openboxGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToOpenboxResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;
            var launchUrl = ScriptHelper.ExtractUrlFromScript(gameLaunchScript, request.Environment);

            return clientResponse.ToOpenboxResult(launchUrl);
        }
    }
}
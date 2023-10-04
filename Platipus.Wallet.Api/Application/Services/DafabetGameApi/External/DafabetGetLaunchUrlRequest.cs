namespace Platipus.Wallet.Api.Application.Services.DafabetGameApi.External;

using System.Text.Json.Serialization;
using Helpers;
using Requests;
using Results.ResultToResultMappers;
using Wallet;

public sealed record DafabetGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] DafabetGetLaunchUrlGameApiRequest ApiRequest)
    : IRequest<IDafabetResult<string>>
{
    public sealed class Handler : IRequestHandler<DafabetGetLaunchUrlRequest, IDafabetResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IDafabetGameApiClient _dafabetGameApiClient;

        public Handler(
            IWalletService walletService,
            IDafabetGameApiClient dafabetGameApiClient)
        {
            _walletService = walletService;
            _dafabetGameApiClient = dafabetGameApiClient;
        }

        public async Task<IDafabetResult<string>> Handle(
            DafabetGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _dafabetGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToDafabetResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;
            var launchUrl = ScriptHelper.ExtractUrlFromScript(gameLaunchScript, request.Environment);

            return clientResponse.ToDafabetResult(launchUrl);
        }
    }
}
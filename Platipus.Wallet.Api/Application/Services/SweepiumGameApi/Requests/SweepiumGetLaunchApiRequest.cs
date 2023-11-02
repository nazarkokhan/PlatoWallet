using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi.Requests;

public sealed record SweepiumGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] SweepiumGetLaunchGameApiRequest ApiRequest)
    : IRequest<ISweepiumResult<string>>
{
    public sealed class Handler : IRequestHandler<SweepiumGetLaunchUrlRequest, ISweepiumResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly ISweepiumGameApiClient _SweepiumGameApiClient;

        public Handler(IWalletService walletService, ISweepiumGameApiClient SweepiumGameApiClient)
        {
            _walletService = walletService;
            _SweepiumGameApiClient = SweepiumGameApiClient;
        }

        public async Task<ISweepiumResult<string>> Handle(
            SweepiumGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _SweepiumGameApiClient.LaunchAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToSweepiumResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;
            var launchUrl = ScriptHelper.ExtractUrlFromScript(gameLaunchScript, request.Environment);

            return clientResponse.ToSweepiumResult(launchUrl);
        }
    }
}
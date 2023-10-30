namespace Platipus.Wallet.Api.Application.Services.EverymatrixGameApi.Requests;

using System.Text.Json.Serialization;
using External;
using Helpers;
using Results.Everymatrix.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record EverymatrixGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] EverymatrixGetLaunchUrlGameApiRequest ApiRequest)
    : IRequest<IEverymatrixResult<string>>
{
    public sealed class Handler : IRequestHandler<EverymatrixGetLaunchUrlRequest, IEverymatrixResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IEverymatrixGameApiClient _everymatrixGameApiClient;

        public Handler(
            IWalletService walletService,
            IEverymatrixGameApiClient everymatrixGameApiClient)
        {
            _walletService = walletService;
            _everymatrixGameApiClient = everymatrixGameApiClient;
        }

        public async Task<IEverymatrixResult<string>> Handle(
            EverymatrixGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _everymatrixGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToEverymatrixResult<string>();

            var launchUrl = clientResponse.Data.Data;

            return clientResponse.ToEverymatrixResult(launchUrl);
        }
    }
}
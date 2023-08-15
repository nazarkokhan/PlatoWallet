namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.External;

using System.Text.RegularExpressions;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Newtonsoft.Json;
using Requests;
using Results.ResultToResultMappers;
using Results.Uranus;
using Results.Uranus.WithData;
using Wallet;

public sealed record UranusGetDemoLaunchGameUrlRequest(
        [property: JsonProperty("environment")] string Environment,
        [property: JsonProperty("apiRequest")] UranusGetDemoLaunchUrlGameApiRequest GameApiRequest)
    : IRequest<IUranusResult<UranusSuccessResponse<UranusGameUrlData>>>
{
    public sealed class Handler : IRequestHandler<UranusGetDemoLaunchGameUrlRequest,
        IUranusResult<UranusSuccessResponse<UranusGameUrlData>>>
    {
        private readonly IWalletService _walletService;
        private readonly IUranusGameApiClient _gameApiClient;

        public Handler(
            IWalletService walletService,
            IUranusGameApiClient gameApiClient)
        {
            _walletService = walletService;
            _gameApiClient = gameApiClient;
        }

        public async Task<IUranusResult<UranusSuccessResponse<UranusGameUrlData>>> Handle(
            UranusGetDemoLaunchGameUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var clientResponse = await _gameApiClient.GetDemoLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                request.GameApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToUranusFailureResult<UranusSuccessResponse<UranusGameUrlData>>();

            var response =
                new UranusSuccessResponse<UranusGameUrlData>(
                    new UranusGameUrlData(clientResponse.Data.Data.Data.Url));

            return UranusResultFactory.Success(response);
        }
    }
}
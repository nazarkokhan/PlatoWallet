namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.External;

using System.Text.Json.Serialization;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Requests;
using Results.ResultToResultMappers;
using Results.Uranus;
using Results.Uranus.WithData;
using Wallet;

public sealed record UranusGetLaunchGameUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] UranusGetLaunchUrlGameApiRequest GameApiRequest)
    : IRequest<IUranusResult<UranusSuccessResponse<UranusGameUrlData>>>

{
    public sealed record Handler : IRequestHandler<UranusGetLaunchGameUrlRequest,
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
            UranusGetLaunchGameUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var clientResponse = await _gameApiClient.GetGameLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                request.GameApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToUranusFailureResult<UranusSuccessResponse<UranusGameUrlData>>();

            var response = new UranusSuccessResponse<UranusGameUrlData>(
                    new UranusGameUrlData(clientResponse.Data.Data.Data.Url));

            return UranusResultFactory.Success(response);
        }
    }
}
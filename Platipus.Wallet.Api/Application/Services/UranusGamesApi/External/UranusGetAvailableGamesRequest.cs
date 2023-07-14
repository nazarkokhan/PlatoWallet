namespace Platipus.Wallet.Api.Application.Services.UranusGamesApi.External;

using System.Text.Json.Serialization;
using Application.Requests.Wallets.Uranus.Base;
using Application.Requests.Wallets.Uranus.Data;
using Requests;
using Results.ResultToResultMappers;
using Results.Uranus;
using Results.Uranus.WithData;
using Wallet;

public sealed record UranusGetAvailableGamesRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] UranusGetAvailableGamesGameApiRequest ApiRequest)
    : IRequest<IUranusResult<UranusSuccessResponse<UranusAvailableGamesData>>>
{
    public sealed class Handler : IRequestHandler<UranusGetAvailableGamesRequest,
        IUranusResult<UranusSuccessResponse<UranusAvailableGamesData>>>
    {
        private readonly IWalletService _walletService;
        private readonly IUranusGameApiClient _gameApiClient;

        public Handler(IWalletService walletService, IUranusGameApiClient gameApiClient)
        {
            _walletService = walletService;
            _gameApiClient = gameApiClient;
        }

        public async Task<IUranusResult<UranusSuccessResponse<UranusAvailableGamesData>>> Handle(
            UranusGetAvailableGamesRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            if (walletResponse.Data is null)
            {
                return walletResponse.ToUranusFailureResult<UranusSuccessResponse<UranusAvailableGamesData>>();
            }

            var clientResponse = await _gameApiClient.GetAvailableGamesAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToUranusFailureResult<UranusSuccessResponse<UranusAvailableGamesData>>();

            var response =
                new UranusSuccessResponse<UranusAvailableGamesData>(
                    new UranusAvailableGamesData(clientResponse.Data?.Data?.Data.Docs!));

            return UranusResultFactory.Success(response);
        }
    }
}
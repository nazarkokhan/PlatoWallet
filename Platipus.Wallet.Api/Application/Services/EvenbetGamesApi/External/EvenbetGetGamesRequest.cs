namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using System.Text.Json.Serialization;
using Application.Requests.Wallets.Evenbet.Models;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record EvenbetGetGamesRequest([property: JsonPropertyName("environment")] string Environment)
    : IRequest<IEvenbetResult<List<EvenbetGameModel>>>
{
    public sealed class Handler : IRequestHandler<EvenbetGetGamesRequest, IEvenbetResult<List<EvenbetGameModel>>>
    {
        private readonly IWalletService _walletService;
        private readonly IEvenbetGameApiClient _evenbetGameApiClient;

        public Handler(
            IWalletService walletService,
            IEvenbetGameApiClient evenbetGameApiClient)
        {
            _walletService = walletService;
            _evenbetGameApiClient = evenbetGameApiClient;
        }

        public async Task<IEvenbetResult<List<EvenbetGameModel>>> Handle(
            EvenbetGetGamesRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            if (walletResponse.Data is null)
            {
                return walletResponse.ToEvenbetFailureResult<List<EvenbetGameModel>>();
            }

            var clientResponse = await _evenbetGameApiClient.GetGamesAsync(
                walletResponse.Data.BaseUrl,
                cancellationToken);

            if (clientResponse.IsFailure || clientResponse.Data?.Data is null)
                return clientResponse.ToEvenbetFailureResult<List<EvenbetGameModel>>();

            var games = clientResponse.Data.Data;
            var response = new List<EvenbetGameModel>(games);

            return clientResponse.ToEvenbetResult(response);
        }
    }
}
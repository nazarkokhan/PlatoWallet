namespace Platipus.Wallet.Api.Application.Services.EvenbetGamesApi.External;

using Newtonsoft.Json;
using Results.Evenbet.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record EvenbetGetGamesRequest([property: JsonProperty("environment")] string Environment)
    : IRequest<IEvenbetResult<EvenbetGetGamesResponse>>
{
    public sealed class Handler : IRequestHandler<EvenbetGetGamesRequest, IEvenbetResult<EvenbetGetGamesResponse>>
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

        public async Task<IEvenbetResult<EvenbetGetGamesResponse>> Handle(
            EvenbetGetGamesRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            if (walletResponse.Data is null)
            {
                return walletResponse.ToEvenbetFailureResult<EvenbetGetGamesResponse>();
            }

            var clientResponse = await _evenbetGameApiClient.GetGamesAsync(
                walletResponse.Data.BaseUrl,
                cancellationToken);

            if (clientResponse.IsFailure || clientResponse.Data?.Data is null)
                return clientResponse.ToEvenbetFailureResult<EvenbetGetGamesResponse>();

            var games = clientResponse.Data.Data.Games;
            var response = new EvenbetGetGamesResponse(games);

            return clientResponse.ToEvenbetResult(response);
        }
    }
}
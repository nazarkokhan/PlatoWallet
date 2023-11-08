namespace Platipus.Wallet.Api.Application.Requests.External.Sw;

using System.Text.Json.Serialization;
using Services.SwGameApi;
using Services.SwGameApi.Requests;
using Services.SwGameApi.Responses;
using Services.Wallet;

public sealed record SwGetGameBetsRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] SwGetGameBetsGameApiRequest ApiRequest)
    : IRequest<IResult<SwGetGameBetsGameApiResponse>>
{
    public sealed class Handler : IRequestHandler<SwGetGameBetsRequest, IResult<SwGetGameBetsGameApiResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly ISwGameApiClient _swGameApiClient;

        public Handler(
            IWalletService walletService,
            ISwGameApiClient swGameApiClient)
        {
            _walletService = walletService;
            _swGameApiClient = swGameApiClient;
        }

        public async Task<IResult<SwGetGameBetsGameApiResponse>> Handle(
            SwGetGameBetsRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _swGameApiClient.GetGameBetsAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure || clientResponse.Data.IsFailure)
                return ResultFactory.Failure<SwGetGameBetsGameApiResponse>(ErrorCode.GameServerApiError);

            var gameBets = clientResponse.Data.Data;

            return ResultFactory.Success(gameBets);
        }
    }
}
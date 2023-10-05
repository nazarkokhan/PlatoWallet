namespace Platipus.Wallet.Api.Application.Requests.External.Uis;

using System.ComponentModel;
using System.Text.Json.Serialization;
using Helpers;
using Services.UisGamesApi;
using Services.UisGamesApi.Dto;
using Services.Wallet;

public sealed record UisGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] UisGetLaunchGameApiRequest ApiRequest)
    : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<UisGetLaunchUrlRequest, IResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IUisGameApiClient _uisGameApiClient;

        public Handler(
            IWalletService walletService,
            IUisGameApiClient uisGameApiClient)
        {
            _walletService = walletService;
            _uisGameApiClient = uisGameApiClient;
        }

        public async Task<IResult<string>> Handle(
            UisGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _uisGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return ResultFactory.Failure<string>(ErrorCode.GameServerApiError);

            var gameLaunchScript = clientResponse.Data.Data;
            var launchUrl = ScriptHelper.ExtractUrlFromScript(gameLaunchScript, request.Environment);

            return ResultFactory.Success(launchUrl);
        }
    }
}
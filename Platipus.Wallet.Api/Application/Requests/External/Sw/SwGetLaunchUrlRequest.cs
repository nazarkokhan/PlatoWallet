namespace Platipus.Wallet.Api.Application.Requests.External.Sw;

using System.Text.Json.Serialization;
using Helpers;
using Services.SwGameApi;
using Services.SwGameApi.Requests;
using Services.Wallet;

public sealed record SwGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] SwGetLaunchUrlGameApiRequest ApiRequest)
    : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<SwGetLaunchUrlRequest, IResult<string>>
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

        public async Task<IResult<string>> Handle(
            SwGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var clientResponse = await _swGameApiClient.GetLaunchScriptAsync(
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
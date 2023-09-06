namespace Platipus.Wallet.Api.Application.Services.SynotGameApi.External;

using System.Text.Json.Serialization;
using Requests;
using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Wallet;

public sealed record SynotGetGameLaunchScriptRequest(
        string Environment,
        [property: JsonPropertyName("apiRequest")] SynotGetGameLaunchScriptGameApiRequest ApiRequest)
    : IRequest<ISynotResult<string>>
{
    public sealed class Handler : IRequestHandler<SynotGetGameLaunchScriptRequest, ISynotResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly ISynotGameApiClient _synotGameApiClient;

        public Handler(IWalletService walletService, ISynotGameApiClient synotGameApiClient)
        {
            _walletService = walletService;
            _synotGameApiClient = synotGameApiClient;
        }

        public async Task<ISynotResult<string>> Handle(
            SynotGetGameLaunchScriptRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var casinoId = request.Environment switch
            {
                "local" => "synot_local",
                "test" or "gameserver-test" or "wbg" => "synot_platipus",
                _ => "synot_stage"
            };
            
            var clientResponse = await _synotGameApiClient.GetGameLaunchScriptAsync(
                casinoId,
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToSynotFailureResult<string>();

            var gameLaunchScript = clientResponse.Data.Data;

            return clientResponse.ToSynotResult(gameLaunchScript);
        }
    }
}
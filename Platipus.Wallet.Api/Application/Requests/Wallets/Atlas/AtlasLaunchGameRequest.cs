namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using Responses.AtlasPlatform;
using Results.Atlas;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;
using Services.AtlasGamesApi;
using Services.AtlasGamesApi.Requests;
using Services.Wallet;

public sealed record AtlasLaunchGameRequest(
    string Environment,
    AtlasGameLaunchGameApiRequest ApiRequest,
    string? Token = null) : 
        IAtlasRequest, IRequest<IAtlasResult<AtlasLaunchGameResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasLaunchGameRequest, IAtlasResult<AtlasLaunchGameResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly IAtlasGameApiClient _atlasGameApiClient;

        public Handler(
            IWalletService walletService, 
            IAtlasGameApiClient atlasGameApiClient)
        {
            _walletService = walletService;
            _atlasGameApiClient = atlasGameApiClient;
        }

        public async Task<IAtlasResult<AtlasLaunchGameResponse>> Handle(
            AtlasLaunchGameRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _atlasGameApiClient.LaunchGameAsync(
                walletResponse.Data.BaseUrl, request.ApiRequest, request.Token!,
                cancellationToken);
            
            if (clientResponse.IsFailure)
                return clientResponse.ToAtlasResult<AtlasLaunchGameResponse>();
            
            var response = new AtlasLaunchGameResponse(clientResponse.Data.Data.Url);
            return AtlasResultFactory.Success(response);
        }
    }
}
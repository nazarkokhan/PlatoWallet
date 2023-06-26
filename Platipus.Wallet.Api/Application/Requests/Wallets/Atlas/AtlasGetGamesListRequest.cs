namespace Platipus.Wallet.Api.Application.Requests.Wallets.Atlas;

using Base;
using FluentValidation;
using Responses.AtlasPlatform;
using Results.Atlas;
using Results.Atlas.WithData;
using Results.ResultToResultMappers;
using Services.AtlasGamesApi;
using Services.AtlasGamesApi.Requests;
using Services.Wallet;

public sealed record AtlasGetGamesListRequest(
    string Environment,
    AtlasGetGamesListGameApiRequest ApiRequest,
    string? Token = null) : 
        IAtlasRequest, IRequest<IAtlasResult<AtlasGetGamesListResponse>>
{
    public sealed class Handler : 
        IRequestHandler<AtlasGetGamesListRequest, IAtlasResult<AtlasGetGamesListResponse>>
    {
        private readonly IAtlasGameApiClient _atlasGameApiClient;
        private readonly IWalletService _walletService;

        public Handler(
            IAtlasGameApiClient atlasGameApiClient, 
            IWalletService walletService)
        {
            _atlasGameApiClient = atlasGameApiClient;
            _walletService = walletService;
        }

        public async Task<IAtlasResult<AtlasGetGamesListResponse>> Handle(
            AtlasGetGamesListRequest request, CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment, cancellationToken);
            
            var clientResponse = await _atlasGameApiClient.GetGamesListAsync(
                walletResponse.Data.BaseUrl, request.ApiRequest, request.Token!,
                cancellationToken);
            
            if (clientResponse.IsFailure)
                return clientResponse.ToAtlasResult<AtlasGetGamesListResponse>();
            
            var response = new AtlasGetGamesListResponse(clientResponse.Data.Data.GameList);
            return AtlasResultFactory.Success(response);
        }
    }

    public sealed class AtlasGetGamesListRequestValidator 
        : AbstractValidator<AtlasGetGamesListRequest>
    {
        public AtlasGetGamesListRequestValidator()
        {
            RuleFor(x => x.Environment).NotEmpty();
        }
    }
}
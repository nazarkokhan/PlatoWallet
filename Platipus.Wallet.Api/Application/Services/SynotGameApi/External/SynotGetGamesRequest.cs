namespace Platipus.Wallet.Api.Application.Services.SynotGameApi.External;

 using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Wallet;

public sealed record SynotGetGamesRequest(string Environment) : IRequest<ISynotResult<SynotGetGamesResponse>>
{
    public sealed class Handler : IRequestHandler<SynotGetGamesRequest, ISynotResult<SynotGetGamesResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly ISynotGameApiClient _synotGameApiClient;

        public Handler(IWalletService walletService, ISynotGameApiClient synotGameApiClient)
        {
            _walletService = walletService;
            _synotGameApiClient = synotGameApiClient;
        }

        public async Task<ISynotResult<SynotGetGamesResponse>> Handle(
            SynotGetGamesRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            var casinoId = request.Environment switch
            {
                "local" => "synot_local",
                "test" or "gameserver-test" => "synot_platipus",
                _ => "synot_stage"
            };

            var clientResponse = await _synotGameApiClient.GetGamesAsync(
                casinoId,
                walletResponse.Data.BaseUrl,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToSynotFailureResult<SynotGetGamesResponse>();

            var gamesResponse = clientResponse.Data.Data;

            return clientResponse.ToSynotResult(gamesResponse);
        }
    }
}
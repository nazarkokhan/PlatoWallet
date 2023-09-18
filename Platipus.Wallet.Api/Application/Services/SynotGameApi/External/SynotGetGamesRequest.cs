namespace Platipus.Wallet.Api.Application.Services.SynotGameApi.External;

using System.Text.Json.Serialization;
using Results.ResultToResultMappers;
using Results.Synot.WithData;
using Wallet;

public sealed record SynotGetGamesRequest(
        string Environment,
        [property: JsonPropertyName("casinoId")] string CasinoId)
    : IRequest<ISynotResult<SynotGetGamesResponse>>
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

            var clientResponse = await _synotGameApiClient.GetGamesAsync(
                request.CasinoId,
                walletResponse.Data.BaseUrl,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToSynotFailureResult<SynotGetGamesResponse>();

            var gamesResponse = clientResponse.Data.Data;

            return clientResponse.ToSynotResult(gamesResponse);
        }
    }
}
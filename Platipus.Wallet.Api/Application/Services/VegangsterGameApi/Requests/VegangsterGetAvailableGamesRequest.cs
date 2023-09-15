namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.Requests;

using Domain.Entities;
using External;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Wallet;

public sealed record VegangsterGetAvailableGamesRequest(
    string Environment,
    string CasinoId,
    VegangsterGetAvailableGamesGameApiRequest ApiRequest) : IRequest<IVegangsterResult<VegangsterGetAvailableGamesResponse>>
{
    public sealed class Handler : IRequestHandler<VegangsterGetAvailableGamesRequest,
        IVegangsterResult<VegangsterGetAvailableGamesResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly IVegangsterGameApiClient _vegangsterGameApiClient;
        private readonly WalletDbContext _walletDbContext;

        public Handler(
            IWalletService walletService,
            IVegangsterGameApiClient vegangsterGameApiClient,
            WalletDbContext walletDbContext)
        {
            _walletService = walletService;
            _vegangsterGameApiClient = vegangsterGameApiClient;
            _walletDbContext = walletDbContext;
        }

        public async Task<IVegangsterResult<VegangsterGetAvailableGamesResponse>> Handle(
            VegangsterGetAvailableGamesRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var casinoXApiSignature = await _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .Select(x => x.Params.VegangsterPrivateWalletSecuritySign)
               .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var clientResponse = await _vegangsterGameApiClient.GetAvailableGamesAsync(
                walletResponse.Data.BaseUrl,
                request.CasinoId,
                casinoXApiSignature!,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToVegangsterFailureResult<VegangsterGetAvailableGamesResponse>();

            var response = new VegangsterGetAvailableGamesResponse(clientResponse.Data.Data.Games);

            return VegangsterResultFactory.Success(response);
        }
    }
}
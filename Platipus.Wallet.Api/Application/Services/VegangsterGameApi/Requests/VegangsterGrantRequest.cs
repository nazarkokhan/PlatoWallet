namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.Requests;

using Domain.Entities;
using External;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Wallet;

public sealed record VegangsterGrantRequest(
    string Environment,
    string CasinoId,
    VegangsterGrantGameApiRequest ApiRequest) : IRequest<IVegangsterResult<VegangsterGrantResponse>>
{
    public sealed class Handler : IRequestHandler<VegangsterGrantRequest,
        IVegangsterResult<VegangsterGrantResponse>>
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

        public async Task<IVegangsterResult<VegangsterGrantResponse>> Handle(
            VegangsterGrantRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var casinoXApiSignature = await _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.CasinoId)
               .Select(x => x.Params.VegangsterPrivateWalletSecuritySign)
               .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var clientResponse = await _vegangsterGameApiClient.GrantAsync(
                walletResponse.Data.BaseUrl,
                request.CasinoId,
                casinoXApiSignature!,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToVegangsterFailureResult<VegangsterGrantResponse>();

            var response = new VegangsterGrantResponse(clientResponse.Data.Data.Id);

            return VegangsterResultFactory.Success(response);
        }
    }
}
namespace Platipus.Wallet.Api.Application.Services.VegangsterGameApi.Requests;

using Domain.Entities;
using External;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.ResultToResultMappers;
using Results.Vegangster;
using Results.Vegangster.WithData;
using Wallet;

public sealed record VegangsterGetDemoLaunchUrlRequest(
    string Environment,
    string OperatorId,
    VegangsterGetDemoLaunchUrlGameApiRequest ApiRequest) : IRequest<IVegangsterResult<VegangsterGetLaunchUrlResponse>>
{
    public sealed class Handler
        : IRequestHandler<VegangsterGetDemoLaunchUrlRequest, IVegangsterResult<VegangsterGetLaunchUrlResponse>>
    {
        private readonly IWalletService _walletService;
        private readonly WalletDbContext _walletDbContext;
        private readonly IVegangsterGameApiClient _vegangsterGameApiClient;

        public Handler(
            IWalletService walletService,
            IVegangsterGameApiClient vegangsterGameApiClient,
            WalletDbContext walletDbContext)
        {
            _walletService = walletService;
            _vegangsterGameApiClient = vegangsterGameApiClient;
            _walletDbContext = walletDbContext;
        }

        public async Task<IVegangsterResult<VegangsterGetLaunchUrlResponse>> Handle(
            VegangsterGetDemoLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var casinoXApiSignature = await _walletDbContext.Set<Casino>()
               .Where(c => c.Id == request.OperatorId)
               .Select(x => x.Params.VegangsterPrivateWalletSecuritySign)
               .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            var clientResponse = await _vegangsterGameApiClient.GetDemoLaunchUrlAsync(
                walletResponse.Data.BaseUrl,
                request.OperatorId,
                casinoXApiSignature!,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToVegangsterFailureResult<VegangsterGetLaunchUrlResponse>();

            var response = new VegangsterGetLaunchUrlResponse(clientResponse.Data.Data.Url);

            return VegangsterResultFactory.Success(response);
        }
    }
}
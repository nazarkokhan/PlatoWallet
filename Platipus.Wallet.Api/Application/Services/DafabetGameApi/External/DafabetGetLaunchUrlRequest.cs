namespace Platipus.Wallet.Api.Application.Services.DafabetGameApi.External;

using System.Text.Json.Serialization;
using Domain.Entities;
using Helpers;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Requests;
using Results.ResultToResultMappers;
using Wallet;

public sealed record DafabetGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] DafabetGetLaunchUrlGameApiRequest ApiRequest)
    : IRequest<IDafabetResult<string>>
{
    public sealed class Handler : IRequestHandler<DafabetGetLaunchUrlRequest, IDafabetResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IDafabetGameApiClient _dafabetGameApiClient;
        private readonly WalletDbContext _walletDbContext;

        public Handler(
            IWalletService walletService,
            IDafabetGameApiClient dafabetGameApiClient,
            WalletDbContext walletDbContext)
        {
            _walletService = walletService;
            _dafabetGameApiClient = dafabetGameApiClient;
            _walletDbContext = walletDbContext;
        }

        public async Task<IDafabetResult<string>> Handle(
            DafabetGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);
            var user = await _walletDbContext.Set<User>()
               .Where(u => u.Username == request.ApiRequest.PlayerId)
               .Select(u => new { CasinoSignatureKey = u.Casino.SignatureKey })
               .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return DafabetResultFactory.Failure<string>(DafabetErrorCode.PlayerNotFound);
            }

            var clientResponse = await _dafabetGameApiClient.GetLaunchScriptAsync(
                walletResponse.Data.BaseUrl,
                user.CasinoSignatureKey,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure)
                return clientResponse.ToDafabetResult<string>();

            var launchUrl = clientResponse.Data.Data;

            return clientResponse.ToDafabetResult(launchUrl);
        }
    }
}
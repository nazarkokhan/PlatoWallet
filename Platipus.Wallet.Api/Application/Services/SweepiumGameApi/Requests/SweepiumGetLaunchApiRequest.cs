using System.Text.Json.Serialization;
using Platipus.Wallet.Api.Application.Helpers;
using Platipus.Wallet.Api.Application.Results.ResultToResultMappers;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;

namespace Platipus.Wallet.Api.Application.Services.SweepiumGameApi.Requests;

using External;

public sealed record SweepiumGetLaunchUrlRequest(
        [property: JsonPropertyName("environment")] string Environment,
        [property: JsonPropertyName("apiRequest")] SweepiumGetLaunchGameApiRequest ApiRequest)
    : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<SweepiumGetLaunchUrlRequest, IResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly ISweepiumGameApiClient _sweepiumGameApiClient;

        public Handler(IWalletService walletService, ISweepiumGameApiClient sweepiumGameApiClient)
        {
            _walletService = walletService;
            _sweepiumGameApiClient = sweepiumGameApiClient;
        }

        public async Task<IResult<string>> Handle(
            SweepiumGetLaunchUrlRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(
                request.Environment,
                cancellationToken);

            var clientResponse = await _sweepiumGameApiClient.LaunchAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure || clientResponse.Data.IsFailure)
                return ResultFactory.Failure<string>(ErrorCode.GameServerApiError);

            var gameLaunchScript = clientResponse.Data.Data;

            return ResultFactory.Success(gameLaunchScript);
        }
    }
}
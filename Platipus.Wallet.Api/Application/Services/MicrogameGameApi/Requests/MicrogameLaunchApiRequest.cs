namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi.Requests;

using System.Text.Json.Serialization;
using External;
using Wallet;

public sealed record MicrogameLaunchApiRequest(
    [property: JsonPropertyName("environment")] string Environment,
    [property: JsonPropertyName("apiRequest")] MicrogameLaunchGameApiRequest ApiRequest) : IRequest<IResult<string>>
{
    public sealed class Handler : IRequestHandler<MicrogameLaunchApiRequest, IResult<string>>
    {
        private readonly IWalletService _walletService;
        private readonly IMicrogameGameApiClient _microgameGameApiClient;

        public Handler(
            IWalletService walletService,
            IMicrogameGameApiClient microgameGameApiClient)
        {
            _walletService = walletService;
            _microgameGameApiClient = microgameGameApiClient;
        }

        public async Task<IResult<string>> Handle(
            MicrogameLaunchApiRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var walletResponse = await _walletService.GetEnvironmentAsync(
                    request.Environment,
                    cancellationToken);

                var clientResponse = await _microgameGameApiClient.LaunchAsync(
                    walletResponse.Data.BaseUrl,
                    request.ApiRequest,
                    cancellationToken);

                if (clientResponse.IsFailure)
                    return ResultFactory.Failure<string>(ErrorCode.GameServerApiError);

                var response = clientResponse.Data.Data;

                return ResultFactory.Success(response);
            }
            catch (Exception e)
            {
                return ResultFactory.Failure<string>(ErrorCode.Unknown, e);
            }
        }
    }
}
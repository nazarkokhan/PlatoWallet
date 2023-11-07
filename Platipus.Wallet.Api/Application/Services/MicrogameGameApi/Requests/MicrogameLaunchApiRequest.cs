namespace Platipus.Wallet.Api.Application.Services.MicrogameGameApi.Requests;

using System.Text.Json.Serialization;
using External;
using Responses;
using Wallet;

public sealed record MicrogameLaunchApiRequest(
    [property: JsonPropertyName("environment")] string Environment,
    [property: JsonPropertyName("apiRequest")] MicrogameLaunchGameApiRequest ApiRequest) : IRequest<
    IResult<MicrogameLaunchGameApiResponse>>
{
    public sealed class Handler : IRequestHandler<MicrogameLaunchApiRequest, IResult<MicrogameLaunchGameApiResponse>>
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

        public async Task<IResult<MicrogameLaunchGameApiResponse>> Handle(
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

                if (clientResponse.IsFailure || clientResponse.Data.IsFailure)
                    return ResultFactory.Failure<MicrogameLaunchGameApiResponse>(ErrorCode.GameServerApiError);

                var response = clientResponse.Data.Data;

                return ResultFactory.Success(response);
            }
            catch (Exception e)
            {
                return ResultFactory.Failure<MicrogameLaunchGameApiResponse>(ErrorCode.Unknown, e);
            }
        }
    }
}
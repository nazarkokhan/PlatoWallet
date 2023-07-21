namespace Platipus.Wallet.Api.Application.Services.AnakatechGamesApi.External;

using System.Text;
using Newtonsoft.Json;
using Requests;
using Results.Anakatech.WithData;
using Results.ResultToResultMappers;
using Wallet;

public sealed record AnakatechLaunchGameRequest(
    [property: JsonProperty("environment")] string Environment,
    [property: JsonProperty("apiRequest")] AnakatechLaunchGameApiRequest ApiRequest) : IRequest<IAnakatechResult<Uri>>
{
    public sealed class Handler : IRequestHandler<AnakatechLaunchGameRequest, IAnakatechResult<Uri>>
    {
        private readonly IWalletService _walletService;
        private readonly IAnakatechGameApiClient _anakatechGameApiClient;

        public Handler(
            IWalletService walletService,
            IAnakatechGameApiClient anakatechGameApiClient)
        {
            _walletService = walletService;
            _anakatechGameApiClient = anakatechGameApiClient;
        }

        public async Task<IAnakatechResult<Uri>> Handle(
            AnakatechLaunchGameRequest request,
            CancellationToken cancellationToken)
        {
            var walletResponse = await _walletService.GetEnvironmentAsync(request.Environment, cancellationToken);

            if (walletResponse.Data is null)
            {
                return walletResponse.ToAnakatechFailureResult<Uri>();
            }

            var clientResponse = await _anakatechGameApiClient.GetLaunchGameUrlAsBytesAsync(
                walletResponse.Data.BaseUrl,
                request.ApiRequest,
                cancellationToken);

            if (clientResponse.IsFailure || clientResponse.Data?.Data is null)
                return clientResponse.ToAnakatechFailureResult<Uri>();

            var gameUrlBytes = await GetBytesFromStream(clientResponse.Data.Data);
            var gameUrl = Encoding.UTF8.GetString(gameUrlBytes);

            var response = new Uri(gameUrl);
            return clientResponse.ToAnakatechResult(response);
        }
        
        public static async Task<byte[]> GetBytesFromStream(Stream stream)
        {
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.External;

using Infrastructure.Persistence;
using Results.Hub88.WithData;
using Services.Hub88GamesApi;
using Services.Hub88GamesApi.DTOs.Requests;

public record ExternalHub88RewardsRequest(Hub88GameApiCreateRewardRequestDto Request)
    : IRequest<IHub88Result<Hub88GameApiCreateRewardResponseDto>>
{
    public class Handler : IRequestHandler<ExternalHub88RewardsRequest, IHub88Result<Hub88GameApiCreateRewardResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IHub88GamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IHub88Result<Hub88GameApiCreateRewardResponseDto>> Handle(
            ExternalHub88RewardsRequest request,
            CancellationToken cancellationToken)
        {
            var casinoGamesResponse = await _gamesApiClient.CreateRewardAsync(
                request.Request,
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}
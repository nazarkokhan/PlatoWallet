namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using Infrastructure.Persistence;
using Results.Hub88.WithData;
using Services.ObsoleteGameApiStyle.Hub88GamesApi;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Responses;

public record ExternalHub88CancelRewardsRequest(Hub88GameApiCancelRewardRequestDto Request)
    : IRequest<IHub88Result<Hub88GameApiCancelRewardResponseDto>>
{
    public class Handler : IRequestHandler<ExternalHub88CancelRewardsRequest, IHub88Result<Hub88GameApiCancelRewardResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IHub88GamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IHub88Result<Hub88GameApiCancelRewardResponseDto>> Handle(
            ExternalHub88CancelRewardsRequest request,
            CancellationToken cancellationToken)
        {
            var casinoGamesResponse = await _gamesApiClient.CancelRewardAsync(
                request.Request,
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Hub88;
using Results.Hub88.WithData;
using Services.ObsoleteGameApiStyle.Hub88GamesApi;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Responses;

public record ExternalHub88PrepaidsListRequest(string CasinoId, string Game)
    : IRequest<IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>>
{
    public class Handler : IRequestHandler<ExternalHub88PrepaidsListRequest, IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IHub88GamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IHub88Result<List<Hub88PrepaidGamesApiResponseDto>>> Handle(
            ExternalHub88PrepaidsListRequest request,
            CancellationToken cancellationToken)
        {
            var game = await _context.Set<Game>()
                .Where(c => c.LaunchName == request.Game)
                .Select(
                    c => new
                    {
                        GameServerId = c.GameServiceId,
                        c.LaunchName
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
                return Hub88ResultFactory.Failure<List<Hub88PrepaidGamesApiResponseDto>>(
                    Hub88ErrorCode.RS_ERROR_TRANSACTION_DOES_NOT_EXIST);

            var gamesApiRequest = new Hub88PrepaidsListGamesApiRequestDto(
                request.CasinoId,
                game.GameServerId,
                game.LaunchName);

            var gamesApiResponse = await _gamesApiClient.GetPrepaidsListAsync(gamesApiRequest, cancellationToken);

            return gamesApiResponse;
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.External.Hub88;

using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Hub88;
using Results.Hub88.WithData;
using Services.ObsoleteGameApiStyle.Hub88GamesApi;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;
using Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Responses;

public record ExternalHub88GetCasinoGamesRequest(string CasinoId) : IRequest<IHub88Result<List<Hub88GetGameDto>>>
{
    public class Handler : IRequestHandler<ExternalHub88GetCasinoGamesRequest, IHub88Result<List<Hub88GetGameDto>>>
    {
        private readonly WalletDbContext _context;
        private readonly IHub88GamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IHub88GamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IHub88Result<List<Hub88GetGameDto>>> Handle(
            ExternalHub88GetCasinoGamesRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(
                    c => c.Id == request.CasinoId
                      && c.Provider == WalletProvider.Hub88)
                .AnyAsync(cancellationToken);

            if (!casinoExist)
                return Hub88ResultFactory.Failure<List<Hub88GetGameDto>>(Hub88ErrorCode.RS_ERROR_INVALID_PARTNER);

            var casinoGamesResponse = await _gamesApiClient.GetGamesListAsync(
                new Hub88GetGamesListRequestDto(request.CasinoId),
                cancellationToken);

            return casinoGamesResponse;
        }
    }
}
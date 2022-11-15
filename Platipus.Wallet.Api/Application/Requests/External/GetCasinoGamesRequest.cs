namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;
using Services.GamesApi;
using Services.GamesApi.DTOs.Responses;

public record GetCasinoGamesRequest(string CasinoId) : IRequest<IPswResult<GetCasinoGamesListResponseDto>>
{
    public class Handler : IRequestHandler<GetCasinoGamesRequest, IPswResult<GetCasinoGamesListResponseDto>>
    {
        private readonly WalletDbContext _context;
        private readonly IGamesApiClient _gamesApiClient;

        public Handler(WalletDbContext context, IGamesApiClient gamesApiClient)
        {
            _context = context;
            _gamesApiClient = gamesApiClient;
        }

        public async Task<IPswResult<GetCasinoGamesListResponseDto>> Handle(
            GetCasinoGamesRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .AnyAsync(cancellationToken);

            if (!casinoExist)
                return PswResultFactory.Failure<GetCasinoGamesListResponseDto>(PswErrorCode.InvalidCasinoId);

            var casinoGamesResponse = await _gamesApiClient.GetCasinoGamesAsync(request.CasinoId, cancellationToken);

            return casinoGamesResponse;
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetCasinoGamesRequest(string? CasinoId) : IRequest<IResult<List<GetCommonGameDto>>>
{
    public class Handler : IRequestHandler<GetCasinoGamesRequest, IResult<List<GetCommonGameDto>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<List<GetCommonGameDto>>> Handle(
            GetCasinoGamesRequest request,
            CancellationToken cancellationToken)
        {
            var casinos = await _context.Set<Game>()
                .Where(
                    c => request.CasinoId == null
                      || c.CasinoGames.Any(g => g.CasinoId == request.CasinoId))
                .Select(
                    c => new GetCommonGameDto(
                        c.Id,
                        c.GameServerId,
                        c.Name,
                        c.LaunchName,
                        c.CategoryId))
                .ToListAsync(cancellationToken);

            return ResultFactory.Success(casinos);
        }
    }
}
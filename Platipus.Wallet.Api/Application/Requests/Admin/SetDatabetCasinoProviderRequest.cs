namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Domain.Entities.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.Factories;

public record SetDatabetCasinoProviderRequest(string CasinoId) : IRequest<Results.Common.Result.IResult>
{
    public class Handler : IRequestHandler<SetDatabetCasinoProviderRequest, Results.Common.Result.IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            SetDatabetCasinoProviderRequest request,
            CancellationToken cancellationToken)
        {
            var databetCasinos = await _context.Set<Casino>()
                .Where(c => c.Provider == CasinoProvider.Dafabet)
                .ToListAsync(cancellationToken);

            foreach (var databetCasino in databetCasinos)
            {
                databetCasino.Provider = null;
            }

            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return ResultFactory.Failure(ErrorCode.InvalidCasinoId);

            casino.Provider = CasinoProvider.Dafabet;

            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}
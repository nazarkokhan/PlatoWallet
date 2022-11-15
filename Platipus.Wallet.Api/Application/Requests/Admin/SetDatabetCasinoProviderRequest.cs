namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record SetDatabetCasinoProviderRequest(string CasinoId) : IRequest<IPswResult>
{
    public class Handler : IRequestHandler<SetDatabetCasinoProviderRequest, IPswResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult> Handle(
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
                return PswResultFactory.Failure(PswErrorCode.InvalidCasinoId);

            casino.Provider = CasinoProvider.Dafabet;

            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}
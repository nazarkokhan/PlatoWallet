namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record CreateCasinoRequest(
    string CasinoId,
    string SignatureKey,
    CasinoProvider Provider,
    List<string> Currencies) : IRequest<IPswResult>
{
    public class Handler : IRequestHandler<CreateCasinoRequest, IPswResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult> Handle(
            CreateCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(e => e.Id == request.CasinoId)
                .AnyAsync(cancellationToken);

            if (casinoExist)
                return PswResultFactory.Failure(PswErrorCode.InvalidCasinoId);

            var supportedCurrencies = await _context.Set<Currency>()
                .ToListAsync(cancellationToken);

            var matchedCurrencies = supportedCurrencies
                .Where(c => request.Currencies.Any(rc => rc == c.Name))
                .ToList();

            if (matchedCurrencies.Count != request.Currencies.Count)
                return PswResultFactory.Failure(PswErrorCode.WrongCurrency);

            if (request.Provider is CasinoProvider.Dafabet)
            {
                var dafabetCasinoExist = await _context.Set<Casino>()
                    .Where(e => e.Provider == request.Provider)
                    .AnyAsync(cancellationToken);

                if (dafabetCasinoExist)
                    return PswResultFactory.Failure(PswErrorCode.InvalidCasinoId);
            }

            var casino = new Casino
            {
                Id = request.CasinoId,
                SignatureKey = request.SignatureKey,
                Provider = request.Provider,
                CasinoCurrencies = matchedCurrencies.Select(c => new CasinoCurrencies {CurrencyId = c.Id})
                    .ToList()
            };

            _context.Add(casino);
            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateCasinoRequest(
    string CasinoId,
    string SignatureKey,
    CasinoProvider Provider,
    List<string> Currencies,
    int? SwProviderId) : IRequest<IPswResult>
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

            switch (request.Provider)
            {
                case CasinoProvider.Dafabet:
                {
                    var dafabetCasinoExist = await _context.Set<Casino>()
                        .Where(e => e.Provider == request.Provider)
                        .AnyAsync(cancellationToken);

                    if (dafabetCasinoExist)
                        return PswResultFactory.Failure(PswErrorCode.InvalidCasinoId);
                    break;
                }
                case CasinoProvider.Sw or CasinoProvider.GamesGlobal or CasinoProvider.SoftBet or CasinoProvider.Uis
                    when request.SwProviderId is null:
                    return PswResultFactory.Failure(PswErrorCode.BadParametersInTheRequest);
            }

            var casino = new Casino
            {
                Id = request.CasinoId,
                SignatureKey = request.SignatureKey,
                Provider = request.Provider,
                SwProviderId = request.Provider is CasinoProvider.Sw or
                                                   CasinoProvider.GamesGlobal or
                                                   CasinoProvider.SoftBet or
                                                   CasinoProvider.Uis
                    ? request.SwProviderId
                    : null,
                CasinoCurrencies = matchedCurrencies
                    .Select(c => new CasinoCurrencies { CurrencyId = c.Id })
                    .ToList()
            };

            _context.Add(casino);
            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}
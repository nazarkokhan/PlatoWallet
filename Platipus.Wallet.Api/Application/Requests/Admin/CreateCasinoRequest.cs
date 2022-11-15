namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record CreateCasinoRequest(
    string CasinoId,
    string SignatureKey,
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

            var casino = new Casino
            {
                Id = request.CasinoId,
                SignatureKey = request.SignatureKey,
                CasinoCurrencies = matchedCurrencies.Select(c => new CasinoCurrencies {CurrencyId = c.Id})
                    .ToList()
            };

            _context.Add(casino);
            await _context.SaveChangesAsync(cancellationToken);

            return PswResultFactory.Success();
        }
    }
}
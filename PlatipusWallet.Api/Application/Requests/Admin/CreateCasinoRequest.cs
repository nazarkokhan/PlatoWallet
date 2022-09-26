namespace PlatipusWallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;

public record CreateCasinoRequest(
    string CasinoId,
    string SignatureKey,
    List<string> Currencies) : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateCasinoRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateCasinoRequest request,
            CancellationToken cancellationToken)
        {
            var casinoExist = await _context.Set<Casino>()
                .Where(e => e.Id == request.CasinoId)
                .AnyAsync(cancellationToken);

            if (casinoExist)
                return ResultFactory.Failure(ErrorCode.InvalidCasinoId);

            var supportedCurrencies = await _context.Set<Currency>()
                .ToListAsync(cancellationToken);

            var matchedCurrencies = supportedCurrencies
                .Where(c => request.Currencies.Any(rc => rc == c.Name))
                .ToList();

            if (matchedCurrencies.Count != request.Currencies.Count)
                return ResultFactory.Failure(ErrorCode.WrongCurrency);

            var casino = new Casino
            {
                Id = request.CasinoId,
                SignatureKey = request.SignatureKey,
                CasinoCurrencies = matchedCurrencies.Select(
                        c => new CasinoCurrencies
                        {
                            CurrencyId = c.Id
                        })
                    .ToList()
            };

            _context.Add(casino);
            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}
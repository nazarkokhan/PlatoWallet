namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using DTOs;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;
using Wallets.Psw.Base.Response;

public record GetCasinoCurrenciesRequest(
    string CasinoId) : IRequest<IPswResult<GetCasinoCurrenciesRequest.Response>>
{
    public class Handler : IRequestHandler<GetCasinoCurrenciesRequest, IPswResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<Response>> Handle(
            GetCasinoCurrenciesRequest request,
            CancellationToken cancellationToken)
        {
            var casino = await _context.Set<Casino>()
                .Where(c => c.Id == request.CasinoId)
                .Select(
                    c => new
                    {
                        Currencies = c.CasinoCurrencies
                            .Select(
                                cu => new GetCurrencyDto(
                                    cu.Currency.Id,
                                    cu.Currency.Name))
                            .ToList()
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (casino is null)
                return PswResultFactory.Failure<Response>(PswErrorCode.InvalidCasinoId);

            var result = new Response(casino.Currencies);

            return PswResultFactory.Success(result);
        }
    }

    public record Response(List<GetCurrencyDto> Items) : PswBaseResponse;
}
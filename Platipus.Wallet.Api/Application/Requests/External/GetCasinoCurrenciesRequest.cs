namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using DTOs;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;
using Wallets.Psw.Base.Response;

public record GetCasinoCurrenciesRequest(
    string CasinoId) : IRequest<IResult<GetCasinoCurrenciesRequest.Response>>
{
    public class Handler : IRequestHandler<GetCasinoCurrenciesRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<Response>> Handle(
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
                return ResultFactory.Failure<Response>(ErrorCode.InvalidCasinoId);

            var result = new Response(casino.Currencies);
            
            return ResultFactory.Success(result);
        }
    }
    
    public record Response(List<GetCurrencyDto> Items) : PswBaseResponse;
}
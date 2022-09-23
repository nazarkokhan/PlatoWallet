namespace PlatipusWallet.Api.Application.Requests.Auth;

using Domain.Entities;
using DTOs;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record GetCasinoCurrenciesRequest(
    string CasinoId) : IRequest<IResult<List<GetCurrencyDto>>>
{
    public class Handler : IRequestHandler<GetCasinoCurrenciesRequest, IResult<List<GetCurrencyDto>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<List<GetCurrencyDto>>> Handle(
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
                return ResultFactory.Failure<List<GetCurrencyDto>>(ErrorCode.InvalidCasinoId);

            return ResultFactory.Success(casino.Currencies);
        }
    }
}
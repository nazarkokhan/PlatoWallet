namespace Platipus.Wallet.Api.Application.Requests.External;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetCurrenciesRequest : IRequest<IResult<GetCurrenciesRequest.Response>>
{
    public class Handler : IRequestHandler<GetCurrenciesRequest, IResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<Response>> Handle(
            GetCurrenciesRequest request,
            CancellationToken cancellationToken)
        {
            var currencies = await _context.Set<Currency>()
                .Select(c => c.Id)
                .ToListAsync(cancellationToken);

            var result = new Response(currencies);

            return ResultFactory.Success(result);
        }
    }

    public record Response(List<string> Items) : BaseResponse;
}
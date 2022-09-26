namespace PlatipusWallet.Api.Application.Requests.External;

using Base.Responses;
using MediatR;
using Microsoft.EntityFrameworkCore;
using DTOs;
using Results.Common;
using Results.Common.Result;
using Results.Common.Result.WithData;
using Domain.Entities;
using Infrastructure.Persistence;

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
    
    public record Response(List<GetCurrencyDto> Items) : BaseResponse;
}
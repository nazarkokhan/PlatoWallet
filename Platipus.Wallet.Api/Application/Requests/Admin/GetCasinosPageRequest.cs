namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Base.Common.Page;
using Domain.Entities;
using Domain.Entities.Enums;
using DTOs;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record GetCasinosPageRequest(
    PageRequest Page,
    string? CasinoId,
    CasinoProvider? Provider) : IRequest<IPswResult<IPage<GetCasinosPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetCasinosPageRequest, IPswResult<IPage<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<IPage<Response>>> Handle(
            GetCasinosPageRequest request,
            CancellationToken cancellationToken)
        {
            var casinosQuery = _context.Set<Casino>()
                .AsNoTracking();

            if (request.CasinoId is not null)
                casinosQuery = casinosQuery
                    .Where(c => c.Id == request.CasinoId);

            if (request.Provider is not null)
                casinosQuery = casinosQuery
                    .Where(c => c.Provider == request.Provider);

            var totalCount = await casinosQuery.CountAsync(cancellationToken);

            casinosQuery = casinosQuery
                .OrderByDescending(p => p.Id);

            var casinos = await casinosQuery
                .SkipTake(request.Page)
                .Select(
                    c => new Response(
                        c.Id,
                        c.SignatureKey,
                        c.Provider,
                        c.CasinoCurrencies
                            .Select(cu => cu.Currency)
                            .Select(cu => new GetCurrencyDto(cu.Id, cu.Name))
                            .ToList()))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(casinos, totalCount);

            return PswResultFactory.Success(page);
        }
    }

    public record Response(
        string Id,
        string SignatureKey,
        CasinoProvider? Provider,
        List<GetCurrencyDto> Currencies);
}
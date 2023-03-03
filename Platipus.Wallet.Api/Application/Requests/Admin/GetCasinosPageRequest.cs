namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Base.Common.Page;
using Domain.Entities;
using Domain.Entities.Enums;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetCasinosPageRequest(
    PageRequest Page,
    string? CasinoId,
    CasinoProvider? Provider) : IRequest<IResult<IPage<GetCasinosPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetCasinosPageRequest, IResult<IPage<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<IPage<Response>>> Handle(
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
                .OrderByDescending(p => p.CreatedDate);

            var casinos = await casinosQuery
                .SkipTake(request.Page)
                .Select(
                    c => new Response(
                        c.Id,
                        c.Provider,
                        c.SignatureKey,
                        c.InternalId,
                        c.GameEnvironmentId,
                        c.Params,
                        c.CasinoCurrencies
                            .Select(cu => cu.CurrencyId)
                            .ToList(),
                        c.CasinoGames
                            .Select(g => g.Game.LaunchName)
                            .ToList()))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(casinos, totalCount);

            return ResultFactory.Success(page);
        }
    }

    public record Response(
        string Id,
        CasinoProvider? Provider,
        string SignatureKey,
        int InternalId,
        string GameEnvironmentId,
        Casino.SpecificParams Params,
        List<string> Currencies,
        List<string> Games);
}
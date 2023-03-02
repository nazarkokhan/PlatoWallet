namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetGameEnvironmentsPageRequest : IRequest<IResult<List<GetGameEnvironmentsPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetGameEnvironmentsPageRequest, IResult<List<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }


        public async Task<IResult<List<Response>>> Handle(
            GetGameEnvironmentsPageRequest request,
            CancellationToken cancellationToken)
        {
            var environments = await _context.Set<GameEnvironment>()
                .Select(
                    g => new Response(
                        g.Id,
                        g.BaseUrl,
                        g.UisBaseUrl,
                        g.Casinos
                            .Select(c => c.Id)
                            .ToList()))
                .ToListAsync(cancellationToken);

            return ResultFactory.Success(environments);
        }
    }

    public record Response(
        string Id,
        Uri BaseUrl,
        Uri UisBaseUrl,
        List<string> CasinosIds);
}
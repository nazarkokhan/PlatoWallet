namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Base.Common.Page;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record GetUserSessionsPageRequest(PageRequest Page, string User)
    : IRequest<IPswResult<IPage<GetUserSessionsPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetUserSessionsPageRequest, IPswResult<IPage<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<IPage<Response>>> Handle(
            GetUserSessionsPageRequest request,
            CancellationToken cancellationToken)
        {
            var usersQuery = _context.Set<User>()
                .AsNoTracking();

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            usersQuery = usersQuery
                .OrderByDescending(p => p.Id);

            var users = await usersQuery
                .SkipTake(request.Page)
                .Select(
                    u => new Response(
                        u.Id,
                        u.Username,
                        u.Balance,
                        u.Currency.Id,
                        u.Sessions
                            .Select(
                                s => new SessionDto(
                                    s.Id,
                                    s.ExpirationDate))
                            .ToList()))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(users, totalCount);

            return PswResultFactory.Success(page);
        }
    }

    public record Response(
        int Id,
        string Username,
        decimal Balance,
        string Currency,
        List<SessionDto> Sessions);

    public record SessionDto(
        string Id,
        DateTime ExpirationDate);
}
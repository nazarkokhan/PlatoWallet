namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Base.Common.Page;
using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetUsersPageRequest(
        string? CasinoId,
        string? Username,
        PageRequest Page)
    : IRequest<IPswResult<IPage<GetUsersPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetUsersPageRequest, IPswResult<IPage<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<IPage<Response>>> Handle(
            GetUsersPageRequest request,
            CancellationToken cancellationToken)
        {
            var usersQuery = _context.Set<User>()
                .AsNoTracking();

            if (request.CasinoId is not null)
                usersQuery = usersQuery.Where(u => u.CasinoId == request.CasinoId);

            if (request.Username is not null)
                usersQuery = usersQuery.Where(u => u.Username == request.Username);

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            usersQuery = usersQuery.OrderByDescending(p => p.CreatedDate);

            var users = await usersQuery
                .SkipTake(request.Page)
                .Select(
                    u => new Response(
                        u.Id,
                        u.Username,
                        u.Password,
                        u.IsDisabled,
                        u.CasinoId,
                        u.Casino.InternalId,
                        u.CurrencyId))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(users, totalCount);

            return PswResultFactory.Success(page);
        }
    }

    public record Response(
        int Id,
        string Username,
        string Password,
        bool IsDisabled,
        string CasinoId,
        int CasinoInternalId,
        string Currency);
}
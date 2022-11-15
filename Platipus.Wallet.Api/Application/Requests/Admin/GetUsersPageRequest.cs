namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Base.Page;
using Domain.Entities;
using DTOs;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;

public record GetUsersPageRequest(PageRequest Page, string CasinoId) : IRequest<IPswResult<IPage<GetUsersPageRequest.Response>>>
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
                .Where(u => u.CasinoId == request.CasinoId)
                .AsNoTracking();

            var totalCount = await usersQuery.CountAsync(cancellationToken);

            usersQuery = usersQuery
                .OrderByDescending(p => p.Id);

            var users = await usersQuery
                .SkipTake(request.Page)
                .Select(
                    u => new Response(
                        u.Id,
                        u.UserName,
                        u.Password,
                        u.IsDisabled,
                        new GetCurrencyDto(u.CurrencyId, u.Currency.Name)))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(users, totalCount);

            return PswResultFactory.Success(page);
        }
    }

    public record Response(
        Guid Id,
        string UserName,
        string Password,
        bool IsDisabled,
        GetCurrencyDto CurrencyDto);
}
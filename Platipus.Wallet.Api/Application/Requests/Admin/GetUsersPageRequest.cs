namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Base.Common.Page;
using Domain.Entities;
using DTOs;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record GetUsersPageRequest(
        string? CasinoId,
        string? UserName,
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

            if (request.UserName is not null)
                usersQuery = usersQuery.Where(u => u.UserName == request.UserName);

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
                        u.SwUserId,
                        u.CasinoId,
                        u.Casino.SwProviderId,
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
        int? SwUserId,
        string CasinoId,
        int? CasinoSwProviderId,
        GetCurrencyDto CurrencyDto);
}
namespace Platipus.Wallet.Api.Application.Requests.Admin;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Extensions;
using Base.Page;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Common.Result.Factories;

public record GetUserSessionsPageRequest(PageRequest Page, string User) : IRequest<IResult<IPage<GetUserSessionsPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetUserSessionsPageRequest, IResult<IPage<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<IPage<Response>>> Handle(
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
                        u.UserName,
                        u.Balance,
                        u.Currency.Name,
                        u.Sessions
                            .Select(
                                s => new SessionDto(
                                    s.Id,
                                    s.ExpirationDate))
                            .ToList()))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(users, totalCount);

            return ResultFactory.Success(page);
        }
    }

    public record Response(
        Guid Id,
        string User,
        decimal Balance,
        string Currency,
        List<SessionDto> Sessions);

    public record SessionDto(
        Guid Id,
        DateTime ExpirationDate);
}
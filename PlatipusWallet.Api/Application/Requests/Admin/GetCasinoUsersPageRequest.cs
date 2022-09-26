namespace PlatipusWallet.Api.Application.Requests.Admin;

using Base.Page;
using Domain.Entities;
using DTOs;
using Extensions;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Results.Common.Result;
using Results.Common.Result.WithData;

public record GetCasinoUsersPageRequest(PageRequest Page, string CasinoId) : IRequest<IResult<IPage<GetCasinoUsersPageRequest.Response>>>
{
    public class Handler : IRequestHandler<GetCasinoUsersPageRequest, IResult<IPage<Response>>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<IPage<Response>>> Handle(
            GetCasinoUsersPageRequest request,
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

            return ResultFactory.Success(page);
        }
    }

    public record Response(
        Guid Id,
        string UserName,
        string Password,
        bool IsDisabled,
        GetCurrencyDto CurrencyDto);
}
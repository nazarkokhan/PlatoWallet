namespace PlatipusWallet.Api.Application.Requests.Test;

using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Page;
using PlatipusWallet.Api.Results.Common.Result;
using PlatipusWallet.Domain.Entities;
using PlatipusWallet.Infrastructure.Persistence;
using Results.Common.Result.WithData;

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
                                    s.ExpirationDate,
                                    s.ErrorMock == null
                                        ? null
                                        : new ErrorMockDto(
                                            s.ErrorMock.Id,
                                            s.ErrorMock.MethodPath,
                                            s.ErrorMock.Body,
                                            s.ErrorMock.HttpStatusCode)))
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
        DateTime ExpirationDate,
        ErrorMockDto? ErrorMock);

    public record ErrorMockDto(
        Guid Id,
        string MethodPath,
        string Body,
        HttpStatusCode HttpStatusCode);
}
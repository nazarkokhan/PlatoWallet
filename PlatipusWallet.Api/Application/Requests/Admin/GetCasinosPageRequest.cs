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

public record GetCasinosPageRequest(PageRequest Page, string CasinoId) : IRequest<IResult<IPage<GetCasinosPageRequest.Response>>>
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

            var totalCount = await casinosQuery.CountAsync(cancellationToken);

            casinosQuery = casinosQuery
                .OrderByDescending(p => p.Id);

            var casinos = await casinosQuery
                .SkipTake(request.Page)
                .Select(
                    c => new Response(
                        c.Id,
                        c.SignatureKey,
                        c.CasinoCurrencies
                            .Select(cu => cu.Currency)
                            .Select(cu => new GetCurrencyDto(cu.Id, cu.Name))
                            .ToList(),
                        c.Users.Select(
                                u => new UserDto(
                                    u.Id,
                                    u.UserName,
                                    u.Password,
                                    u.IsDisabled,
                                    new GetCurrencyDto(u.CurrencyId, u.Currency.Name)))
                            .ToList()))
                .ToListAsync(cancellationToken);

            var page = new Page<Response>(casinos, totalCount);

            return ResultFactory.Success(page);
        }
    }

    public record Response(
        string Id,
        string SignatureKey,
        List<GetCurrencyDto> Currencies,
        List<UserDto> Users);

    public record UserDto(
        Guid Id,
        string UserName,
        string Password,
        bool IsDisabled,
        GetCurrencyDto CurrencyDto);
}
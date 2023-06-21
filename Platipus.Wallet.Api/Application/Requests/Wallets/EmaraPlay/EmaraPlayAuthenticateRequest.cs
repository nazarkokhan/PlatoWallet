using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayAuthenticateRequest(
        string Token, string Provider, string Game, string? Ip = null)
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraplayAuthenticateResponse>>
{
    public sealed class Handler 
        : IRequestHandler<EmaraPlayAuthenticateRequest, IEmaraPlayResult<EmaraplayAuthenticateResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context) => _context = context;

        public async Task<IEmaraPlayResult<EmaraplayAuthenticateResponse>> Handle(
            EmaraPlayAuthenticateRequest request, CancellationToken cancellationToken)
        {

            var user = await _context.Set<User>()
                .AsNoTracking()
                .Include(c => c.Casino)
                .ThenInclude(cg => cg.CasinoGames)
                .Where(u => u.Sessions.Any(s => s.Id == request.Token))
                    .Select(
                    u => new
                    {
                        u.Id,
                        u.Balance,
                        Currency = u.CurrencyId,
                        u.Username
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return EmaraPlayResultFactory.Failure<EmaraplayAuthenticateResponse>(
                    EmaraPlayErrorCode.PlayerNotFound);

            var authenticateResult = new EmaraPlayAuthenticateResult(
                user.Id.ToString(), user.Currency, user.Username, user.Balance);
            var response = new EmaraplayAuthenticateResponse(authenticateResult);
            return EmaraPlayResultFactory.Success(response);
        }
    }
}
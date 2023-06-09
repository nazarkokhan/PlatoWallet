using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;
using Result = Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base.Result;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayAuthenticateRequest(
        string Token, string Provider, string Game, string? Ip = null)
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBaseResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayAuthenticateRequest, IEmaraPlayResult<EmaraPlayBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context) => _context = context;

        public async Task<IEmaraPlayResult<EmaraPlayBaseResponse>> Handle(EmaraPlayAuthenticateRequest request, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                    .Where(u => u.Sessions.Any(s => s.Id == request.Token) && 
                                u.Casino.CasinoGames.Any(c => 
                                    c.Game.Name == request.Game) && 
                                u.Casino.Provider.ToString() == request.Provider)
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
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.BadParameters);

            var result = new Result
            {
                User = user.Id.ToString(),
                Username = user.Username,
                Currency = user.Currency,
                Balance = user.Balance.ToString(CultureInfo.InvariantCulture)
            };
            
            var response = new EmaraPlayBaseResponse(
                "0",
                "Unknown",
                result);

            return EmaraPlayResultFactory.Success(response);
        }
    }
}
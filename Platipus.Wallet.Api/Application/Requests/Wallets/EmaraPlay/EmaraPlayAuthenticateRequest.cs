using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Infrastructure.Persistence;
using Result = Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base.Result;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayAuthenticateRequest(
        string Token, string Provider, string Game, string? Ip = null)
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBaseResponse>>
{
    public sealed class Handler 
        : IRequestHandler<EmaraPlayAuthenticateRequest, IEmaraPlayResult<EmaraPlayBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context) => _context = context;

        public async Task<IEmaraPlayResult<EmaraPlayBaseResponse>> Handle(EmaraPlayAuthenticateRequest request, CancellationToken cancellationToken)
        {
            //TODO it should not match this enum, just check it in security filter to match casino custom properties
            if (!Enum.TryParse(request.Provider, out CasinoProvider provider))
            {
                return EmaraPlayResultFactory.Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.ProviderNotFound);
            }

            var user = await _context.Set<User>()
                .AsNoTracking()
                .Where(u => u.Sessions.Any(s => s.Id == request.Token) &&
                            u.Casino.CasinoGames.Any(c => c.Game.Name == request.Game) &&
                            u.Casino.Provider == provider)
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

            //TODO better put common logic inside dto, dont use magic strings
            //TODO as i see in documentation, all wallet responses follow the same wrapper logic with generic Result property
            var response = new EmaraPlayBaseResponse(
                ((int)EmaraPlayErrorCode.Success).ToString(),
                "Success",
                result);

            return EmaraPlayResultFactory.Success(response);
        }
    }
}
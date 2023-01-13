// ReSharper disable NotAccessedPositionalProperty.Global
namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

using System.Globalization;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.EmaraPlay;
using Results.EmaraPlay.WithData;
using static Results.EmaraPlay.EmaraPlayResultFactory;

public record EmaraPlayBalanceRequest(
        string User,
        string Provider,
        string Token)
    : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBaseResponse>>
{
    public class Handler : IRequestHandler<EmaraPlayBalanceRequest, IEmaraPlayResult<EmaraPlayBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBaseResponse>> Handle(
            EmaraPlayBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Token), cancellationToken: cancellationToken);

            if (session is null)
            {
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerAuthenticationFailed);
            }

            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(u => u.Id == new Guid(request.User))
                .Include(u => u.Currency)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.Balance,
                        s.IsDisabled,
                        // s.Country,
                        s.UserName,
                        Currency = s.Currency.Name
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
            {
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerNotFound);
            }

            if (user.IsDisabled)
                return Failure<EmaraPlayBaseResponse>(EmaraPlayErrorCode.PlayerIsFrozen);

            var response = new EmaraPlayBaseResponse(
                "0",
                EmaraPlayErrorCode.Success.ToString(),
                new Result(
                    Currency: user.Currency,
                    Balance: user.Balance.ToString(CultureInfo.InvariantCulture),
                    Bonus: null));

            return Success(response);
        }
    }
}
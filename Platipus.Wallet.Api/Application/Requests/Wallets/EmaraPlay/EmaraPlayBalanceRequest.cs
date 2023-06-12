using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayBalanceRequest(
        string Provider, string Token, string User) 
        : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBalanceResponse>>
{
    public sealed class Handler : 
        IRequestHandler<EmaraPlayBalanceRequest, IEmaraPlayResult<EmaraPlayBalanceResponse>>
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(WalletDbContext walletDbContext, ILogger<Handler> logger)
        {
            _walletDbContext = walletDbContext;
            _logger = logger;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBalanceResponse>> Handle(
            EmaraPlayBalanceRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _walletDbContext.Set<User>()
                    .TagWith("GetBalance")
                    .Where(u => u.Username == request.User && 
                                u.Sessions.Any(s => s.Id == request.Token))
                    .Select(u => new
                    {
                        Currency = u.Currency.Id,
                        u.Balance,
                        u.Casino.Provider,
                    })
                    .ToListAsync(cancellationToken);
                
                var matchedUser = user.FirstOrDefault(u => 
                    u.Provider.ToString() == request.Provider);
                
                if (matchedUser is null)
                    return EmaraPlayResultFactory.Failure<EmaraPlayBalanceResponse>(EmaraPlayErrorCode.PlayerNotFound);

                var balanceResult = new BalanceResult(matchedUser.Balance.ToString(CultureInfo.InvariantCulture), matchedUser.Currency);
                var response = new EmaraPlayBalanceResponse(
                    ((int)EmaraPlayErrorCode.Success).ToString(),
                    EmaraPlayErrorCode.Success.ToString(),
                    balanceResult);

                return EmaraPlayResultFactory.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Common wallet service GetBalance unknown exception");
                return EmaraPlayResultFactory.Failure<EmaraPlayBalanceResponse>(EmaraPlayErrorCode.InternalServerError, e);
            }
        }
    }
}
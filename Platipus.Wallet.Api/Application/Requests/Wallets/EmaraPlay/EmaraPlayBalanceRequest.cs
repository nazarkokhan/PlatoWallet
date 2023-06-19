using System.Globalization;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Base;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Responses;
using Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay.Results;
using Platipus.Wallet.Api.Application.Results.EmaraPlay;
using Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Domain.Entities.Enums;
using Platipus.Wallet.Infrastructure.Persistence;

namespace Platipus.Wallet.Api.Application.Requests.Wallets.EmaraPlay;

public sealed record EmaraPlayBalanceRequest(
    string Provider,
    string Token,
    string User) : IEmaraPlayBaseRequest, IRequest<IEmaraPlayResult<EmaraPlayBalanceResponse>>
{
    public sealed class Handler : IRequestHandler<EmaraPlayBalanceRequest, IEmaraPlayResult<EmaraPlayBalanceResponse>>
    {
        private readonly WalletDbContext _walletDbContext;
        private readonly ILogger<Handler> _logger;

        public Handler(WalletDbContext walletDbContext, ILogger<Handler> logger)
        {
            _walletDbContext = walletDbContext;
            _logger = logger;
        }

        public async Task<IEmaraPlayResult<EmaraPlayBalanceResponse>> Handle(
            EmaraPlayBalanceRequest request,
            CancellationToken cancellationToken)
        {
            try //TODO we will need to fix exception behaviour
            {
                //TODO it
                if (!Enum.TryParse(request.Provider, out CasinoProvider provider))
                {
                    return EmaraPlayResultFactory.Failure<EmaraPlayBalanceResponse>(EmaraPlayErrorCode
                        .ProviderNotFound);
                }

                //just use
                // var walletResult = await _walletService.GetBalanceAsync(request.Token)
                var user = await _walletDbContext.Set<User>()
                    .AsNoTracking()
                    .TagWith("GetBalance")
                    .Where(
                        u => u.Username == request.User
                             && u.Sessions.Any(s => s.Id == request.Token)
                             && u.Casino.Provider == provider)
                    .Select(
                        u => new
                        {
                            Currency = u.Currency.Id,
                            u.Balance,
                            u.Casino.Provider,
                        })
                    .FirstOrDefaultAsync(cancellationToken);

                if (user is null)
                    return EmaraPlayResultFactory.Failure<EmaraPlayBalanceResponse>(EmaraPlayErrorCode.PlayerNotFound);

                // TODO if your contract with numbers is as strings just put it inside json settings to not repeat yourself
                var balanceResult =
                    new BalanceResult(user.Balance.ToString(CultureInfo.InvariantCulture), user.Currency);
                var response = new EmaraPlayBalanceResponse(
                    ((int)EmaraPlayErrorCode.Success).ToString(),
                    EmaraPlayErrorCode.Success.ToString(),
                    balanceResult);

                return EmaraPlayResultFactory.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Common wallet service GetBalance unknown exception");
                return EmaraPlayResultFactory.Failure<EmaraPlayBalanceResponse>(EmaraPlayErrorCode.InternalServerError,
                    e);
            }
        }
    }
}
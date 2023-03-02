namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix;

using System.Globalization;
using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Everymatrix;
using Results.Everymatrix.WithData;

public record EverymatrixAuthenticateRequest(
        string LaunchToken,
        string RequestScope)
    : IRequest<IEverymatrixResult<EverymatrixAuthenticateRequest.EverymatrixAuthenticationResponse>>, IEveryMatrixBaseRequest
{
    public class Handler : IRequestHandler<EverymatrixAuthenticateRequest, IEverymatrixResult<EverymatrixAuthenticationResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IEverymatrixResult<EverymatrixAuthenticationResponse>> Handle(
            EverymatrixAuthenticateRequest request,
            CancellationToken cancellationToken)
        {
            var launchSession = await _context.Set<Session>()
                .Where(s => s.Id == request.LaunchToken)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.ExpirationDate,
                        s.IsTemporaryToken,
                        User = new
                        {
                            s.User.Balance,
                            Currency = s.User.Currency.Id,
                            s.User.Username,
                            s.User.Id
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (launchSession is null || launchSession.ExpirationDate < DateTime.UtcNow || !launchSession.IsTemporaryToken)
                return EverymatrixResultFactory.Failure<EverymatrixAuthenticationResponse>(EverymatrixErrorCode.TokenNotFound);

            var user = launchSession.User;
            var gameSession = new Session { UserId = user.Id };

            _context.Add(gameSession);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new EverymatrixAuthenticationResponse(
                gameSession.Id,
                user.Balance.ToString(CultureInfo.InvariantCulture),
                user.Currency,
                user.Username,
                user.Id.ToString());

            return EverymatrixResultFactory.Success(response);
        }
    }

    public record EverymatrixAuthenticationResponse(
        string Token,
        string TotalBalance,
        string Currency,
        string UserName,
        string UserId,
        string Country = "Ukraine",
        string Age = "26",
        string Sex = "male") : EverymatrixBaseSuccessResponse;
}
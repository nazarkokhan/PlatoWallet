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
        Guid LaunchToken,
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
                            Currency = s.User.Currency.Name,
                            s.User.UserName,
                            s.User.Id
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (launchSession is null || launchSession.ExpirationDate < DateTime.UtcNow || !launchSession.IsTemporaryToken)
                return EverymatrixResultFactory.Failure<EverymatrixAuthenticationResponse>(EverymatrixErrorCode.TokenNotFound);

            var user = launchSession.User;
            var gameSession = new Session
            {
                Id = Guid.NewGuid(),
                UserId = user.Id
            };

            _context.Add(gameSession);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new EverymatrixAuthenticationResponse(
                gameSession.Id,
                user.Balance.ToString(CultureInfo.InvariantCulture),
                user.Currency,
                user.UserName,
                user.Id);

            return EverymatrixResultFactory.Success(response);
        }
    }

    public record EverymatrixAuthenticationResponse(
        Guid Token,
        string TotalBalance,
        string Currency,
        string UserName,
        Guid UserId,
        string Country = "Ukraine",
        string Age = "26",
        string Sex = "male") : EverymatrixBaseSuccessResponse;
}
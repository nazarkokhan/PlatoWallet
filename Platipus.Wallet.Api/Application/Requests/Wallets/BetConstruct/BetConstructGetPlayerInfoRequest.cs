namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Domain.Entities.Enums;
using Infrastructure.Persistence;
using Results.BetConstruct;
using Results.BetConstruct.WithData;
using static Results.BetConstruct.BetConstructResultFactory;
using Microsoft.EntityFrameworkCore;

public record BetConstructGetPlayerInfoRequest(
    DateTime Time,
    object Data,
    string Hash,
    string Token) : IRequest<IBetConstructResult<BetConstructPlayerInfoResponse>>, IBetConstructBaseRequest
{
    public class Handler : IRequestHandler<BetConstructGetPlayerInfoRequest, IBetConstructResult<BetConstructPlayerInfoResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }


        public async Task<IBetConstructResult<BetConstructPlayerInfoResponse>> Handle(
            BetConstructGetPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var isValidHash = BetConstructVerifyHashExtension.VerifyBetConstructHash(
                request,
                request.Data.ToString(),
                request.Time);

            if (!isValidHash)
            {
                return Failure<BetConstructPlayerInfoResponse>(BetConstructErrorCode.AuthenticationFailed);
            }

            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Token), cancellationToken: cancellationToken);

            if (session is null)
            {
                return Failure<BetConstructPlayerInfoResponse>(BetConstructErrorCode.TokenNotFound);
            }

            //TODO add to all requests
            if (session.ExpirationDate < DateTime.UtcNow)
            {
                return Failure<BetConstructPlayerInfoResponse>(BetConstructErrorCode.TokenExpired);
            }

            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(u => u.Id == session.UserId)
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
                return Failure<BetConstructPlayerInfoResponse>(
                    BetConstructErrorCode.IncorrectParametersPassed,
                    new Exception("User isn't found"));
            }

            if (user.IsDisabled)
                return Failure<BetConstructPlayerInfoResponse>(BetConstructErrorCode.GameIsBlocked, new Exception("User is blocked"));

            var response = new BetConstructPlayerInfoResponse(
                true,
                null,
                null,
                user.Currency,
                user.Balance,
                user.UserName,
                (int)BetConstructGender.Male,
                "Country",
                user.Id.ToString());

            return Success(response);
        }
    }

}
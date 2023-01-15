

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Betflag;
using Results.Betflag.WithData;

public record BetflagBalanceRequest(
    string Key,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBaseResponse>>, IBetflagBaseRequest
{
    public class Handler : IRequestHandler<BetflagBalanceRequest, IBetflagResult<BetflagBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBaseResponse>> Handle(
            BetflagBalanceRequest request,
            CancellationToken cancellationToken)
        {

            var session = await _context.Set<Session>()
                .FirstOrDefaultAsync(s => s.Id == new Guid(request.Key), cancellationToken: cancellationToken);

            if (session is null)
            {
                return BetflagResultFactory.Failure<BetflagBaseResponse>(BetflagErrorCode.InvalidToken);
            }

            var user = await _context.Set<User>()
                .Where(u => u.Id == session.UserId)
                .Include(u => u.Currency)
                .Select(
                    u => new
                    {
                        u.Id,
                        u.Balance,
                        Currency = u.Currency.Name,
                        u.UserName,
                    })
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);

            if (user is null)
            {
                return BetflagResultFactory.Failure<BetflagBaseResponse>(
                    BetflagErrorCode.InvalidParameter,
                    new Exception("The launch token is not valid"));
            }

            var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var hash = BetflagRequestHash.Compute("0", timeStamp).ToUpperInvariant();

            var response = new BetflagBaseResponse(
                (int) BetflagErrorCode.SUCCSESS,
                BetflagErrorCode.SUCCSESS.ToString(),
                (double) user.Balance,
                false,
                user.Currency,
                "IdTicket",
                session.Id.ToString(),
                user.Id.ToString(),
                user.UserName,
                timeStamp,
                hash);
            return BetflagResultFactory.Success(response);
        }
    }
}
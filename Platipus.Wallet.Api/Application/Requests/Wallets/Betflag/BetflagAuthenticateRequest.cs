// ReSharper disable NotAccessedPositionalProperty.Global

namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Api.Extensions.SecuritySign;
using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Betflag;
using Results.Betflag.WithData;

public record BetflagAuthenticateRequest(
    string Key,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagBaseResponse>>, IBetflagBaseRequest
{
    public class Handler : IRequestHandler<BetflagAuthenticateRequest, IBetflagResult<BetflagBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<BetflagBaseResponse>> Handle(
            BetflagAuthenticateRequest request,
            CancellationToken cancellationToken)
        {
            //TODO Key is provided by betflag to user, but key is not user id
            //TODO need to add bets table to db
            //TODO check cancel requests, last transaction.max by must be transaction doesn't exist
            //TODO check how is round find, sometimes it must be find by transaction


            var session = await _context.Set<Session>()
                .LastOrDefaultAsync(s => s.Id == new Guid( request.Key), cancellationToken: cancellationToken);

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

            var hash = BetflagRequestHash.Compute(session.Id.ToString(), timeStamp, "BetflagSecretKey");

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

    public string Key { get; set; }
}
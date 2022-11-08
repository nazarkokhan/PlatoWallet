namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

/*
 5.2 Verify Player "{\"token\":\"b8247e72f7054eaea5f450a374b5356f\"}"
 
 5.3 Get player information "{\"token\":\"b8247e72f7054eaea5f450a374b5356f\"}"
 
 5.4 Get player balance "{\"token\":\"b8247e72f7054eaea5f450a374b5356f\"}"
 
 5.5 Money Transactions "{\
 "token\":\"b8247e72f7054eaea5f450a374b5356f\",\
 "game_uid\":\"hello_slot\",\
 "order_uid\":\"db3cbfdfc9e84be3a79645921aecea8f\",\
 "order_type\":1,\
 "order_amount\":12345}"

 5.6 Cancel Transaction "{\
 "token\":\"b8247e72f7054eaea5f450a374b5356f\",\
 "game_uid\":\"hello_slot\",\
 "game_cycle_uid\":\"c1234e72f7054eaea5f450a374b5356f\",\
 "order_uid_cancel\":\"b4321e72f7054eaea5f450a374b5356f\",\
 "order_uid\":\"z8974e72f7054eaea5f450a374b5356f\"}"

 5.7 Logout "{\"token\":\"b8247e72f7054eaea5f450a374b5356f\"}"

 5.8 Keep token alive "{\"token\":\"b8247e72f7054eaea5f450a374b5356f\"}"

 5.9 Get Game History GET
*/

public record OpenboxBalanceRequest(
        Guid Token,
        OpenboxSingleRequest Request)
    : OpenboxBaseRequest(Token, Request), IRequest<IOpenboxResult<OpenboxBalanceResponse>>
{
    public class Handler : IRequestHandler<OpenboxBalanceRequest, IOpenboxResult<OpenboxBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<OpenboxBalanceResponse>> Handle(
            OpenboxBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(
                    u => u.Sessions
                        .Select(s => s.Id)
                        .Contains(request.Token))
                .Select(
                    s => new
                    {
                        s.Id,
                        s.Balance,
                        s.IsDisabled
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.TokenRelatedErrors);

            if (user.IsDisabled)
                return OpenboxResultFactory.Failure<OpenboxBalanceResponse>(OpenboxErrorCode.TokenRelatedErrors);

            var response = new OpenboxBalanceResponse(user.Balance);

            return OpenboxResultFactory.Success(response);
        }
    }
}
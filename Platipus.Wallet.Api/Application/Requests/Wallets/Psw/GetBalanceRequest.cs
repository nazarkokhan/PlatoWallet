namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw;

using Base;
using Base.Response;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;

public record GetBalanceRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game) : PswBaseRequest(SessionId, User), IRequest<IResult<PswBalanceResponse>>
{
    public class Handler : IRequestHandler<GetBalanceRequest, IResult<PswBalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<PswBalanceResponse>> Handle(
            GetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(
                    u => u.UserName == request.User
                         && u.Sessions
                             .Select(s => s.Id)
                             .Contains(request.SessionId))
                .Select(
                    s => new
                    {
                        s.Id,
                        s.Balance,
                        s.IsDisabled
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.InvalidUser);

            if (user.IsDisabled)
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.UserDisabled);

            var response = new PswBalanceResponse(user.Balance);

            return ResultFactory.Success(response);
        }
    }
}
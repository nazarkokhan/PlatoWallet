namespace Platipus.Wallet.Api.Application.Requests.Wallet;

using Base.Requests;
using Base.Responses;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence;
using Results.Common;
using Results.Common.Result.Factories;
using Results.Common.Result.WithData;

public record GetBalanceRequest(
    Guid SessionId,
    string User,
    string Currency,
    string Game) : PswBaseRequest(SessionId, User), IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<GetBalanceRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            GetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .TagWith("GetUserBalance")
                .Where(u => u.UserName == request.User &&
                            u.Sessions
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
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.InvalidUser);
            
            if (user.IsDisabled)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserDisabled);

            var response = new BalanceResponse(user.Balance);

            return ResultFactory.Success(response);
        }
    }
}
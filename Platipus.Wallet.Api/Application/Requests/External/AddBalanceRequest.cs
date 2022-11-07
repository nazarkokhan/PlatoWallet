namespace Platipus.Wallet.Api.Application.Requests.External;

using Microsoft.EntityFrameworkCore;
using Base.Responses;
using Domain.Entities;
using Results.Common;
using FluentValidation;
using Infrastructure.Persistence;
using Results.Common.Result.Factories;

public record AddBalanceRequest(
    Guid SessionId,
    decimal Balance) : IRequest<IResult<BaseResponse>>
{
    public class Handler : IRequestHandler<AddBalanceRequest, IResult<BaseResponse>>
    {
        private readonly WalletDbContext _context;
        
        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BaseResponse>> Handle(
            AddBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .Where(s => s.Id == request.SessionId)
                .Select(s => new
                {
                    s.Id,
                    s.ExpirationDate,
                    s.User
                })
                .FirstAsync(cancellationToken);

            var user = session.User;
            if (user.IsDisabled)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserDisabled);

            user.Balance += request.Balance;
            
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);
            
            var result = new BalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }
    
    public class Validator : AbstractValidator<AddBalanceRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Balance)
                .ScalePrecision(2, 38);
        }
    }
}
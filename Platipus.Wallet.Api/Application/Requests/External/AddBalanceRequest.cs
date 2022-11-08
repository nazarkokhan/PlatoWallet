namespace Platipus.Wallet.Api.Application.Requests.External;

using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Results.Psw;
using Results.Psw.WithData;
using Wallets.Psw.Base.Response;

public record AddBalanceRequest(
    Guid SessionId,
    decimal Balance) : IRequest<IResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<AddBalanceRequest, IResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;
        
        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<PswBaseResponse>> Handle(
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
                return ResultFactory.Failure<PswBalanceResponse>(ErrorCode.UserDisabled);

            user.Balance += request.Balance;
            
            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);
            
            var result = new PswBalanceResponse(user.Balance);

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
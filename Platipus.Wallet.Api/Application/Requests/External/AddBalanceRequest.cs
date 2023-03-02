namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using DTO;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record AddBalanceRequest(
    string SessionId,
    decimal Balance) : IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<AddBalanceRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            AddBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .Where(s => s.Id == request.SessionId)
                .Select(
                    s => new
                    {
                        s.Id,
                        s.ExpirationDate,
                        s.User
                    })
                .FirstAsync(cancellationToken);

            var user = session.User;
            if (user.IsDisabled)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserIsDisabled);

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
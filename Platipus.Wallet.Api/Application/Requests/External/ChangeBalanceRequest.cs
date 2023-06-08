namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using DTO;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record ChangeBalanceRequest(
    string Username,
    decimal Balance) : IRequest<IResult<BalanceResponse>>
{
    public class Handler : IRequestHandler<ChangeBalanceRequest, IResult<BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult<BalanceResponse>> Handle(
            ChangeBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>()
                .Where(s => s.Username == request.Username)
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserNotFound);
            if (user.IsDisabled)
                return ResultFactory.Failure<BalanceResponse>(ErrorCode.UserIsDisabled);

            user.Balance += request.Balance;

            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var result = new BalanceResponse(user.Balance);

            return ResultFactory.Success(result);
        }
    }

    public class Validator : AbstractValidator<ChangeBalanceRequest>
    {
        public Validator()
        {
            RuleFor(p => p.Balance)
                .PrecisionScale(28, 2, false);
        }
    }
}
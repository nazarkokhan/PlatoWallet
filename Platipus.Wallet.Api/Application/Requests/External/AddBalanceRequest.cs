namespace Platipus.Wallet.Api.Application.Requests.External;

using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Psw;
using Wallets.Psw.Base.Response;

public record AddBalanceRequest(
    Guid SessionId,
    decimal Balance) : IRequest<IPswResult<PswBaseResponse>>
{
    public class Handler : IRequestHandler<AddBalanceRequest, IPswResult<PswBaseResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IPswResult<PswBaseResponse>> Handle(
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
                return PswResultFactory.Failure<PswBalanceResponse>(PswErrorCode.UserDisabled);

            user.Balance += request.Balance;

            _context.Update(user);

            await _context.SaveChangesAsync(cancellationToken);

            var result = new PswBalanceResponse(user.Balance);

            return PswResultFactory.Success(result);
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
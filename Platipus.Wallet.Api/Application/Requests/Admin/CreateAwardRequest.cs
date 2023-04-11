namespace Platipus.Wallet.Api.Application.Requests.Admin;

using Domain.Entities;
using Extensions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record CreateAwardRequest(
    string Username,
    string AwardId,
    DateTime ValidUntil,
    string Game) : IRequest<IResult>
{
    public class Handler : IRequestHandler<CreateAwardRequest, IResult>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IResult> Handle(
            CreateAwardRequest request,
            CancellationToken cancellationToken)
        {
            var awardExists = await _context.Set<Award>()
                .Where(u => u.Id == request.AwardId)
                .AnyAsync(cancellationToken);
            if (awardExists)
                return ResultFactory.Failure(ErrorCode.AwardAlreadyExists);

            var user = await _context.Set<User>()
                .Where(u => u.Username == request.AwardId)
                .FirstOrDefaultAsync(cancellationToken);
            if (user.IsNullOrDisabled(out var userNullOrDisabledResult))
                return userNullOrDisabledResult;

            var award = new Award(request.AwardId, request.ValidUntil) { UserId = user.Id };
            _context.Add(award);

            await _context.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }
}
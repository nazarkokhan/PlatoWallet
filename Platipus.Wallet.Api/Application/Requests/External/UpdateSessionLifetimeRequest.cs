namespace Platipus.Wallet.Api.Application.Requests.External;

using System.Text.Json.Serialization;
using Domain.Entities;
using FluentValidation;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public sealed record UpdateSessionLifetimeRequest([property: JsonPropertyName("sessionId")] string SessionId) : IRequest<IResult>
{
    public sealed class Handler : IRequestHandler<UpdateSessionLifetimeRequest, IResult>
    {
        private readonly WalletDbContext _walletDbContext;

        public Handler(WalletDbContext walletDbContext)
        {
            _walletDbContext = walletDbContext;
        }

        public async Task<IResult> Handle(
            UpdateSessionLifetimeRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _walletDbContext.Set<Session>()
               .Where(s => s.Id == request.SessionId)
               .SingleOrDefaultAsync(cancellationToken);

            if (session is null)
            {
                return ResultFactory.Failure(ErrorCode.SessionNotFound);
            }

            if (session.IsTemporaryToken is false)
            {
                return ResultFactory.Failure(ErrorCode.SessionAlreadyNotTemporary);
            }

            session.IsTemporaryToken = false;

            _walletDbContext.Update(session);

            await _walletDbContext.SaveChangesAsync(cancellationToken);

            return ResultFactory.Success();
        }
    }

    public sealed class UpdateSessionLifetimeRequestValidator : AbstractValidator<UpdateSessionLifetimeRequest>
    {
        public UpdateSessionLifetimeRequestValidator()
        {
            RuleFor(x => x.SessionId)
               .NotEmpty()
               .MinimumLength(10);
        }
    }
}
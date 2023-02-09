namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record OpenboxVerifyPlayerRequest(Guid Token) : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxTokenResponse>>
{
    public class Handler : IRequestHandler<OpenboxVerifyPlayerRequest, IOpenboxResult<OpenboxTokenResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<OpenboxTokenResponse>> Handle(
            OpenboxVerifyPlayerRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == request.Token)
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return OpenboxResultFactory.Failure<OpenboxTokenResponse>(OpenboxErrorCode.TokenRelatedErrors);

            if (session.ExpirationDate <= DateTime.UtcNow)
                return OpenboxResultFactory.Failure<OpenboxTokenResponse>(OpenboxErrorCode.TokenRelatedErrors);

            //TODO session.IsMainToken = false;
            var newSession = new Session {UserId = session.UserId};
            _context.Add(newSession);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new OpenboxTokenResponse(newSession.Id);

            return OpenboxResultFactory.Success(response);
        }
    }
}
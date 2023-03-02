namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record OpenboxVerifyPlayerRequest(string Token) : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxTokenResponse>>
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
            var temporarySession = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == request.Token)
                .FirstOrDefaultAsync(cancellationToken);

            if (temporarySession is null || temporarySession.ExpirationDate < DateTime.UtcNow)
                return OpenboxResultFactory.Failure<OpenboxTokenResponse>(OpenboxErrorCode.TokenRelatedErrors);

            var playSession = new Session { UserId = temporarySession.UserId };
            _context.Add(playSession);

            await _context.SaveChangesAsync(cancellationToken);

            var response = new OpenboxTokenResponse(playSession.Id);

            return OpenboxResultFactory.Success(response);
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Base.Response;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxVerifyPlayerRequest(
        Guid Token,
        OpenboxSingleRequest Request)
    : OpenboxBaseRequest(Token, Request), IRequest<IOpenboxResult<OpenboxTokenResponse>>
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
                .Select(
                    s => new
                    {
                        s.Id,
                        s.ExpirationDate,
                        s.User.UserName
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return OpenboxResultFactory.Failure<OpenboxTokenResponse>(OpenboxErrorCode.TokenRelatedErrors);

            if (session.ExpirationDate <= DateTime.UtcNow)
                return OpenboxResultFactory.Failure<OpenboxTokenResponse>(OpenboxErrorCode.TokenRelatedErrors);
                    //TODO return new token
            var response = new OpenboxTokenResponse(session.Id);

            return OpenboxResultFactory.Success(response);
        }
    }
}
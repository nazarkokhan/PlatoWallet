namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxLogoutRequest(string Token)
    : OpenboxBaseRequest(Token), IRequest<IOpenboxResult<OpenboxLogoutRequest.Response>>
{
    public class Handler : IRequestHandler<OpenboxLogoutRequest, IOpenboxResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<Response>> Handle(OpenboxLogoutRequest request, CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == new Guid(request.Token))
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return OpenboxResultFactory.Failure<Response>(OpenboxErrorCode.TokenRelatedErrors);

            _context.Remove(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new Response();

            return OpenboxResultFactory.Success(response);
        }
    }

    public record Response;
}
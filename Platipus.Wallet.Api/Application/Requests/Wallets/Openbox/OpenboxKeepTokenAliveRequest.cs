namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public record OpenboxKeepTokenAliveRequest(string Token)
    : IOpenboxBaseRequest, IRequest<IOpenboxResult<OpenboxKeepTokenAliveRequest.Response>>
{
    public class Handler : IRequestHandler<OpenboxKeepTokenAliveRequest, IOpenboxResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<Response>> Handle(
            OpenboxKeepTokenAliveRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == request.Token)
                .FirstOrDefaultAsync(cancellationToken);

            if (session is null)
                return OpenboxResultFactory.Failure<Response>(OpenboxErrorCode.TokenRelatedErrors);

            session.ExpirationDate = session.ExpirationDate.AddMinutes(35).ToUniversalTime();

            _context.Update(session);
            await _context.SaveChangesAsync(cancellationToken);

            var response = new Response();

            return OpenboxResultFactory.Success(response);
        }
    }

    public record Response;
}
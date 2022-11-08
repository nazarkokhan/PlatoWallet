namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Base;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxGetPlayerInfoRequest(
        Guid Token,
        OpenboxSingleRequest Request)
    : OpenboxBaseRequest(Token, Request), IRequest<IOpenboxResult<OpenboxGetPlayerInfoRequest.Response>>
{
    public class Handler : IRequestHandler<OpenboxGetPlayerInfoRequest, IOpenboxResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<Response>> Handle(
            OpenboxGetPlayerInfoRequest request,
            CancellationToken cancellationToken)
        {
            var session = await _context.Set<Session>()
                .TagWith("GetSession")
                .Where(u => u.Id == request.Token)
                .Select(
                    s => new
                    {
                        s.Id,
                        User = new
                        {
                            s.User.Id,
                            s.User.UserName,
                            Currency = s.User.Currency.Name
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);
            
            //TODO validate should be 2 token
            if (session is null)
                return OpenboxResultFactory.Failure<Response>(OpenboxErrorCode.TokenRelatedErrors);

            var response = new Response(
                session.User.Id,
                session.User.UserName,
                //TODO session.User.Currency
                0);

            return OpenboxResultFactory.Success(response);
        }
    }

    public record Response(Guid MemberUid, string MemberAccount, int CurrencyCode);
}
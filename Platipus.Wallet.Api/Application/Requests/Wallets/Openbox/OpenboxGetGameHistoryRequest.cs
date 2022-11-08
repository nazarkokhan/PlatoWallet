namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox;

using Infrastructure.Persistence;
using Results.Openbox;
using Results.Openbox.WithData;

public record OpenboxGetGameHistoryRequest(
        Guid AgencyUid,
        Guid MemberUid,
        Guid GameUid,
        Guid GameCycleUid)
    : IRequest<IOpenboxResult<OpenboxGetGameHistoryRequest.Response>>
{
    public class Handler : IRequestHandler<OpenboxGetGameHistoryRequest, IOpenboxResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IOpenboxResult<Response>> Handle(
            OpenboxGetGameHistoryRequest request,
            CancellationToken cancellationToken)
        {
            var response = new Response("http://domain.com/game/history/url?token=123");

            return OpenboxResultFactory.Success(response);
        }
    }

    public record Response(string Url);
}
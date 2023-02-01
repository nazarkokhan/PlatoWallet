namespace Platipus.Wallet.Api.Application.Requests.Wallets.Betflag;

using Base;
using Infrastructure.Persistence;
using Results.Betflag;
using Results.Betflag.WithData;

public record BetflagSessionCloseRequest(
    string IdTicket,
    int TotalBet,
    int TotalWin,
    double TotalJPQuota,
    int TotalWinJP,
    int TotalRound,
    long Timestamp,
    string Hash,
    string ApiName) : IRequest<IBetflagResult<BetflagSessionCloseRequest.Response>>, IBetflagBaseRequest
{
    public class Handler : IRequestHandler<BetflagSessionCloseRequest, IBetflagResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IBetflagResult<Response>> Handle(
            BetflagSessionCloseRequest request,
            CancellationToken cancellationToken)
        {
            var response = new Response();

            return BetflagResultFactory.Success(response);
        }
    }

    public record Response : BetflagBaseResponse;
}
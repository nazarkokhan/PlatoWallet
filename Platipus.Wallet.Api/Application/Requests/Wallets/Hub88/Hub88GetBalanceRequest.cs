namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Infrastructure.Persistence;
using Results.Hub88;
using Results.Hub88.WithData;

public record Hub88GetBalanceRequest(
    string SupplierUser,
    string Token,
    string RequestUuid,
    int GameId,
    string GameCode) : Hub88BaseRequest(SupplierUser, Token, RequestUuid), IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88GetBalanceRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88GetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var response = new Hub88BalanceResponse(
                0,
                request.SupplierUser,
                request.RequestUuid,
                "request.Currency");

            return Hub88ResultFactory.Success(response);
        }
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Infrastructure.Persistence;
using Results.Hub88;
using Results.Hub88.WithData;

public record Hub88RollbackRequest(
    string SupplierUser,
    string TransactionUuid,
    string Token,
    bool RoundClosed,
    string Round,
    string RequestUuid,
    string ReferenceTransactionUuid,
    int GameId,
    string GameCode,
    Hub88MetaDto Meta) : Hub88BaseRequest(SupplierUser, Token, RequestUuid), IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88RollbackRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88RollbackRequest request,
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
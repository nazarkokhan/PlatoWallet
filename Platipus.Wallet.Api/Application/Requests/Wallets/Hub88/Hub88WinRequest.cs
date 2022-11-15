namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Infrastructure.Persistence;
using Results.Hub88;
using Results.Hub88.WithData;

public record Hub88WinRequest(
    string SupplierUser,
    string TransactionUuid,
    string Token,
    bool RoundClosed,
    string Round,
    string RewardUuid,
    string RequestUuid,
    string ReferenceTransactionUuid,
    bool IsFree,
    int GameId,
    string GameCode,
    string Currency,
    string Bet,
    int Amount,
    Hub88MetaDto Meta) : Hub88BaseRequest(SupplierUser, Token, RequestUuid), IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88WinRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88WinRequest request,
            CancellationToken cancellationToken)
        {
            var response = new Hub88BalanceResponse(
                0,
                request.SupplierUser,
                request.RequestUuid,
                request.Currency);

            return Hub88ResultFactory.Success(response);
        }
    }
}
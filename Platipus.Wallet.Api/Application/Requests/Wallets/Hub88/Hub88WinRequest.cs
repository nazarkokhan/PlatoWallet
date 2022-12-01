namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88;

using Base;
using Base.Response;
using Extensions;
using Results.Hub88;
using Results.Hub88.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record Hub88WinRequest(
    string SupplierUser,
    string TransactionUuid,
    Guid Token,
    bool RoundClosed,
    string Round,
    string? RewardUuid,
    string RequestUuid,
    string ReferenceTransactionUuid,
    bool IsFree,
    int GameId,
    string GameCode,
    string Currency,
    string? Bet,
    int Amount,
    Hub88MetaDto? Meta) : IHub88BaseRequest, IRequest<IHub88Result<Hub88BalanceResponse>>
{
    public class Handler : IRequestHandler<Hub88WinRequest, IHub88Result<Hub88BalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IHub88Result<Hub88BalanceResponse>> Handle(
            Hub88WinRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new WinRequest(
                    r.Token,
                    r.SupplierUser,
                    r.Currency,
                    r.GameCode,
                    r.Round,
                    r.TransactionUuid,
                    r.RoundClosed,
                    r.Amount / 100000m));

            var walletResult = await _wallet.WinAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                walletResult.ToHub88Result();

            var response = walletResult.Data.Map(
                d => new Hub88BalanceResponse(
                    (int)(d.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return Hub88ResultFactory.Success(response);
        }
    }
}
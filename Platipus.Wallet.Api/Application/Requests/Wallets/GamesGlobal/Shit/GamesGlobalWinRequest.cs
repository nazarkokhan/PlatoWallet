namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Shit;

using Base;
using Base.Response;
using Extensions;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record GamesGlobalWinRequest(
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
    int Amount) : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalBalanceResponse>>
{
    public class Handler : IRequestHandler<GamesGlobalWinRequest, IGamesGlobalResult<GamesGlobalBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<GamesGlobalBalanceResponse>> Handle(
            GamesGlobalWinRequest request,
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
                walletResult.ToGamesGlobalResult();

            var response = walletResult.Data.Map(
                d => new GamesGlobalBalanceResponse(
                    (int)(d.Balance * 100000),
                    request.SupplierUser,
                    request.RequestUuid,
                    d.Currency));

            return GamesGlobalResultFactory.Success(response);
        }
    }
}
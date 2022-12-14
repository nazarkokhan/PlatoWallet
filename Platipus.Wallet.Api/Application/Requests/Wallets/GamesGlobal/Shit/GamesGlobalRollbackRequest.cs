namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Shit;

using Base;
using Base.Response;
using Extensions;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record GamesGlobalRollbackRequest(
    string SupplierUser,
    string TransactionUuid,
    Guid Token,
    bool RoundClosed,
    string Round,
    string RequestUuid,
    string ReferenceTransactionUuid,
    int GameId,
    string GameCode,
    GamesGlobalMetaDto? Meta) : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalBalanceResponse>>
{
    public class Handler : IRequestHandler<GamesGlobalRollbackRequest, IGamesGlobalResult<GamesGlobalBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<GamesGlobalBalanceResponse>> Handle(
            GamesGlobalRollbackRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(
                r => new RollbackRequest(
                    r.Token,
                    r.SupplierUser,
                    r.GameCode,
                    r.Round,
                    r.TransactionUuid));

            var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
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
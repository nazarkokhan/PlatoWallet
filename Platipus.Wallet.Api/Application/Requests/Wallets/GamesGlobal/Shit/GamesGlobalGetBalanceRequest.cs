namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Shit;

using Base;
using Base.Response;
using Extensions;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record GamesGlobalGetBalanceRequest(
    string SupplierUser,
    Guid Token,
    string RequestUuid,
    int GameId,
    string GameCode) : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalBalanceResponse>>
{
    public class Handler : IRequestHandler<GamesGlobalGetBalanceRequest, IGamesGlobalResult<GamesGlobalBalanceResponse>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<GamesGlobalBalanceResponse>> Handle(
            GamesGlobalGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var walletRequest = request.Map(r => new GetBalanceRequest(r.Token, r.SupplierUser));

            var walletResult = await _wallet.GetBalanceAsync(walletRequest, cancellationToken);
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
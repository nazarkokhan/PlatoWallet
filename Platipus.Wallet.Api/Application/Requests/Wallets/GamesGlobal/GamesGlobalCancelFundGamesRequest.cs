namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Services.Wallet;

public record GamesGlobalCancelFundGameDto
{
    [XmlRpcMember("userInfo")]
    public GamesGlobalUserInfoDto UserInfo { get; init; }

    [XmlRpcMember("requestItemId")]
    public string RequestItemId { get; init; }

    [XmlRpcMember("externalProductId")]
    public int ExternalProductId { get; init; }
}

public record GamesGlobalCancelFundGamesRequest(GamesGlobalCancelFundGameDto[] Cancels)
    : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalCancelFundGamesRequest.Response>>

{
    public class Handler : IRequestHandler<GamesGlobalCancelFundGamesRequest, IGamesGlobalResult<Response>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalCancelFundGamesRequest request,
            CancellationToken cancellationToken)
        {
            // var walletRequest = request.Map(
            //     r => new BetRequest(
            //         r.Token,
            //         r.SupplierUser,
            //         r.Currency,
            //         r.GameCode,
            //         r.Round,
            //         r.TransactionUuid,
            //         r.RoundClosed,
            //         r.Amount / 100000m));
            //
            // var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            // if (walletResult.IsFailure)
            //     walletResult.ToGamesGlobalResult();
            //
            // var response = walletResult.Data.Map(
            //     d => new GamesGlobalBalanceResponse(
            //         (int)(d.Balance * 100000),
            //         request.SupplierUser,
            //         request.RequestUuid,
            //         d.Currency));

            var response = new Response(new RespDto[] { }, new ErrorDto[] { });
            return GamesGlobalResultFactory.Success(response);
        }
    }

    public record Response(
        [property: XmlRpcMember("resps")] RespDto[] Resps,
        [property: XmlRpcMember("errors")] ErrorDto[] Errors);

    public record RespDto([property: XmlRpcMember("requestItemId")] string RequestItemId);

    public record ErrorDto(
        [property: XmlRpcMember("errorCode")] int ErrorCode,
        [property: XmlRpcMember("errorDescription")] string ErrorDescription,
        [property: XmlRpcMember("requestItemId")] string RequestItemId,
        [property: XmlRpcMember("userInfo")] GamesGlobalUserInfoDto UserInfo);
}
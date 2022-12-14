namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Services.Wallet;

public record GamesGlobalServerIdDto
{
    [XmlRpcMember("serverId")]
    public int ServerId { get; init; }
}

public record GamesGlobalCompleteGameRequest(
        GamesGlobalGameInfoDto GameInfo,
        GamesGlobalServerIdDto[] ServerIds)
    : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalCompleteGameRequest.Response>>
{
    public class Handler : IRequestHandler<GamesGlobalCompleteGameRequest, IGamesGlobalResult<Response>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalCompleteGameRequest request,
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

    public record RespDto([property: XmlRpcMember("serverId")] int ServerId);

    public record ErrorDto(
        [property: XmlRpcMember("serverId")] int ServerId,
        [property: XmlRpcMember("errorCode")] int ErrorCode,
        [property: XmlRpcMember("outstandingBetTickets")] string[] OutstandingBetTickets);
}
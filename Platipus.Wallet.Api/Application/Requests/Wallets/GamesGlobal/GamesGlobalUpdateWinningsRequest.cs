namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Services.Wallet;

public record GamesGlobalUpdateWinningDto
{
    [XmlRpcMember("serverId")]
    public int ServerId { get; init; }

    [XmlRpcMember("betTicket")]
    public string BetTicket { get; init; }

    [XmlRpcMember("creditAmt")]
    public long CreditAmt { get; init; }

    [XmlRpcMember("bonusAmt")]
    public long BonusAmt { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("punchTicket")]
    public bool? PunchTicket { get; init; }

    [XmlRpcMember("requestItemId")]
    public string RequestItemId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("externalProductId")]
    public int? ExternalProductId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("freeGames")]
    public bool? FreeGames { get; init; }
}

public record GamesGlobalUpdateWinningsRequest(GamesGlobalUpdateWinningDto[] Winnings)
    : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalUpdateWinningsRequest.Response>>
{
    public class Handler : IRequestHandler<GamesGlobalUpdateWinningsRequest, IGamesGlobalResult<Response>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalUpdateWinningsRequest request,
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

    public record RespDto(
        [property: XmlRpcMember("userInfo")] GamesGlobalUserInfoDto UserInfo,
        [property: XmlRpcMember("betTicket")] string BetTicket,
        [property: XmlRpcMember("cashBal")] long CashBal,
        [property: XmlRpcMember("bonusBal")] long BonusBal,
        [property: XmlRpcMember("gameCashBal")] long GameCashBal,
        [property: XmlRpcMember("gameBonusBal")] long GameBonusBal,
        [property: XmlRpcMember("cashPayout")] long CashPayout,
        [property: XmlRpcMember("bonusPayout")] long BonusPayout,
        [property: XmlRpcMember("requestItemId")] string RequestItemId);

    public record ErrorDto(
        [property: XmlRpcMember("serverId")] int ServerId,
        [property: XmlRpcMember("betTicket")] string BetTicket,
        [property: XmlRpcMember("errorCode")] int ErrorCode,
        [property: XmlRpcMember("requestItemId")] string RequestItemId);
}
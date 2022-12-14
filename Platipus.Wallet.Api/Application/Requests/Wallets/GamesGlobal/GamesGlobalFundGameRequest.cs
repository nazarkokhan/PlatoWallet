namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Services.Wallet;

public record GamesGlobalGameInfoDto
{
    [XmlRpcMember("gameName")]
    public string GameName { get; init; }

    [XmlRpcMember("gameId")]
    public long GameId { get; init; }

    [XmlRpcMember("externalProductId")]
    public int ExternalProductId { get; init; }
}

public record GamesGlobalFundGameDto
{
    [XmlRpcMember("userInfo")]
    public GamesGlobalUserInfoDto UserInfo { get; init; }

    [XmlRpcMember("gameCurrency")]
    public string GameCurrency { get; init; }

    [XmlRpcMember("debitAmt")]
    public long DebitAmt { get; init; }

    [XmlRpcMember("debitMult")]
    public int DebitMult { get; init; }

    [XmlRpcMember("bonusAmt")]
    public long BonusAmt { get; init; }

    [XmlRpcMember("requestItemId")]
    public string RequestItemId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("offerId")]
    public int? OfferId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("instanceId")]
    public int? InstanceId { get; init; }
}

public record GamesGlobalFundGameRequest(
        GamesGlobalGameInfoDto GameInfo,
        GamesGlobalFundGameDto[] Funds)
    : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalFundGameRequest.Response>>
{
    public class Handler : IRequestHandler<GamesGlobalFundGameRequest, IGamesGlobalResult<Response>>
    {
        private readonly IWalletService _wallet;

        public Handler(IWalletService wallet)
        {
            _wallet = wallet;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalFundGameRequest request,
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
        [property: XmlRpcMember("betAmt")] long BetAmt,
        [property: XmlRpcMember("bonusBetAmt")] long BonusBetAmt,
        [property: XmlRpcMember("requestItemId")] string RequestItemId,
        [property: XmlRpcMember("playerGroups")] GamesGlobalPlayerGroupDto[] PlayerGroups);

    public record ErrorDto;
    // [property: XmlRpcMember("serverId")] int ServerId,
    // [property: XmlRpcMember("betTicket")] string BetTicket,
    // [property: XmlRpcMember("errorCode")] int ErrorCode,
    // [property: XmlRpcMember("requestItemId")] string RequestItemId);
}
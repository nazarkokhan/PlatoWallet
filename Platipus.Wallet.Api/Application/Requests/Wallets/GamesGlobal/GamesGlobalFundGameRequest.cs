namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;

using Base;
using Domain.Entities;
using Horizon.XmlRpc.Core;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Results.GamesGlobal;
using Results.GamesGlobal.WithData;
using Results.ResultToResultMappers;
using Services.Wallet;
using Services.Wallet.DTOs;

public record GamesGlobalGameInfoDto
{
    [XmlRpcMember("gameName")]
    public string GameName { get; init; }

    [XmlRpcMember("gameId")]
    public long GameId { get; init; }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    [XmlRpcMember("externalProductId")]
    public int? ExternalProductId { get; init; }
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
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalFundGameRequest request,
            CancellationToken cancellationToken)
        {
            var fund = request.Funds.First();
            var gameInfo = request.GameInfo;
            var userInfo = fund.UserInfo;

            var user = await _context.Set<User>()
                .Where(u => u.SwUserId == userInfo.UserId)
                .Select(u => new { u.UserName })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.InvalidUserId);

            var betTicket = Guid.NewGuid().ToString();
            var walletRequest = new BetRequest(
                Guid.Empty,
                user.UserName,
                fund.GameCurrency,
                gameInfo.GameId.ToString(),
                betTicket,
                false,
                fund.DebitAmt / 100m);

            var walletResult = await _wallet.BetAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToGamesGlobalResult<Response>();
            var responseFund = walletResult.Data;

            var bal = (long)(responseFund.Balance * 100);
            var resp = new RespDto(
                fund.UserInfo,
                betTicket,
                bal,
                0,
                bal,
                0,
                fund.DebitAmt,
                0,
                fund.RequestItemId,
                new GamesGlobalPlayerGroupDto[] { });

            var response = new Response(new[] { resp }, new ErrorDto[] { });
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
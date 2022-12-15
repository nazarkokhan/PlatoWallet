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
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalUpdateWinningsRequest request,
            CancellationToken cancellationToken)
        {
            var winning = request.Winnings.First();

            var user = await _context.Set<Transaction>()
                .Where(t => t.Id == winning.BetTicket)
                // .Select(t => t.Round.User)
                .Select(
                    u => new
                    {
                        u.RoundId,
                        u.Round.User.UserName,
                        UserId = u.Round.User.SwUserId,
                        ServerId = u.Round.User.Casino.SwProviderId
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.InvalidTicketId);

            var betTicket = Guid.NewGuid().ToString();
            var walletRequest = new WinRequest(
                Guid.Empty,
                user.UserName,
                "USD", //TODO save currency?
                "",
                user.RoundId,
                betTicket,
                winning.PunchTicket ?? false,
                winning.CreditAmt / 100m);

            var walletResult = await _wallet.WinAsync(walletRequest, cancellationToken);
            if (walletResult.IsFailure)
                return walletResult.ToGamesGlobalResult<Response>();

            var responseFund = walletResult.Data;
            var bal = (long)(responseFund.Balance * 100);

            var resp = new RespDto(
                new GamesGlobalUserInfoDto()
                {
                    UserId = user.UserId!.Value,
                    UserName = user.UserName,
                    ServerId = user.ServerId!.Value
                },
                betTicket,
                bal,
                0,
                bal,
                0,
                winning.CreditAmt,
                0,
                winning.RequestItemId);

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
        [property: XmlRpcMember("cashPayout")] long CashPayout,
        [property: XmlRpcMember("bonusPayout")] long BonusPayout,
        [property: XmlRpcMember("requestItemId")] string RequestItemId);

    public record ErrorDto(
        [property: XmlRpcMember("serverId")] int ServerId,
        [property: XmlRpcMember("betTicket")] string BetTicket,
        [property: XmlRpcMember("errorCode")] int ErrorCode,
        [property: XmlRpcMember("requestItemId")] string RequestItemId);
}
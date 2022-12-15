namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal;

using Base;
using Domain.Entities;
using Horizon.XmlRpc.Core;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
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
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalCompleteGameRequest request,
            CancellationToken cancellationToken)
        {
            var gameInfo = request.GameInfo;

            var game = await _context.Set<Round>()
                .Where(r => r.Id == gameInfo.GameId.ToString())
                .Select(r => new { r.Finished })
                .FirstOrDefaultAsync(cancellationToken);

            if (game is null)
                return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.GameSettingsNotFound);

            if (!game.Finished)
                return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.UnresolvedTicketsOnCompleteGame);

            var resp = new RespDto(request.ServerIds.FirstOrDefault()?.ServerId ?? 0);

            var response = new Response(new[] { resp }, new ErrorDto[] { });
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
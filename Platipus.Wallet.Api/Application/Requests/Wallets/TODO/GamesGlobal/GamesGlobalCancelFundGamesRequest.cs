namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.GamesGlobal;
using Platipus.Wallet.Api.Application.Results.GamesGlobal.WithData;
using Platipus.Wallet.Api.Application.Services.Wallet;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

public record GamesGlobalCancelFundGamesRequest(GamesGlobalCancelFundGameDto[] Cancels)
    : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalCancelFundGamesRequest.Response>>

{
    public class Handler : IRequestHandler<GamesGlobalCancelFundGamesRequest, IGamesGlobalResult<Response>>
    {
        private readonly IWalletService _wallet;
        private readonly WalletDbContext _context;

        public Handler(IWalletService wallet, WalletDbContext context)
        {
            _wallet = wallet;
            _context = context;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalCancelFundGamesRequest request,
            CancellationToken cancellationToken)
        {
            var cancel = request.Cancels.First();
            var userInfo = cancel.UserInfo;

            var user = await _context.Set<User>()
                .Where(u => u.Id == userInfo.UserId)
                .Select(u => new { UserName = u.Username })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.InvalidUserId);

            return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.GameSettingsNotFound);

            // var betTicket = Guid.NewGuid().ToString();
            // var walletRequest = new RollbackRequest(
            //     Guid.Empty,
            //     user.UserName,
            //     "USD",
            //     gameInfo.GameId.ToString(),
            //     betTicket,
            //     false,
            //     fund.DebitAmt / 100m);
            //
            // var walletResult = await _wallet.RollbackAsync(walletRequest, cancellationToken);
            // if (walletResult.IsFailure)
            //     return walletResult.ToGamesGlobalResult<Response>();

            // var resp = new RespDto(cancel.RequestItemId);
            //
            // var response = new Response(new[] { resp }, new ErrorDto[] { });
            // return GamesGlobalResultFactory.Success(response);
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
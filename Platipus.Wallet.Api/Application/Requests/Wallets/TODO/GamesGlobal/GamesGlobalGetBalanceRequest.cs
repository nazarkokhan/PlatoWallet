namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal;

using Base;
using Horizon.XmlRpc.Core;
using Microsoft.EntityFrameworkCore;
using Platipus.Wallet.Api.Application.Results.GamesGlobal;
using Platipus.Wallet.Api.Application.Results.GamesGlobal.WithData;
using Platipus.Wallet.Domain.Entities;
using Platipus.Wallet.Infrastructure.Persistence;

public record GamesGlobalGetBalanceRequest(GamesGlobalGetBalanceDto[] GetBalances)
    : IGamesGlobalBaseRequest, IRequest<IGamesGlobalResult<GamesGlobalGetBalanceRequest.Response>>

{
    public class Handler : IRequestHandler<GamesGlobalGetBalanceRequest, IGamesGlobalResult<Response>>
    {
        private readonly WalletDbContext _context;

        public Handler(WalletDbContext context)
        {
            _context = context;
        }

        public async Task<IGamesGlobalResult<Response>> Handle(
            GamesGlobalGetBalanceRequest request,
            CancellationToken cancellationToken)
        {
            var r = request.GetBalances.First();

            var user = await _context.Set<User>()
                .Where(u => u.Id == r.UserInfo.UserId)
                .Select(
                    u => new
                    {
                        UserName = u.Username,
                        Currency = u.Currency.Id,
                        u.Balance,
                        SwUserId = u.Id,
                        Casino = new
                        {
                            Id = u.Casino.Id,
                            SwProviderId = u.Casino.InternalId,
                            u.Casino.Provider,
                        }
                    })
                .FirstOrDefaultAsync(cancellationToken);

            if (user is null)
                return GamesGlobalResultFactory.Failure<Response>(GamesGlobalErrorCode.UnknownServerError);

            var bal = (long)(user.Balance * 100);
            var resp = new RespDto(
                r.UserInfo,
                bal,
                0,
                bal,
                0,
                0);
            var response = new Response(new[] { resp }, new ErrorDto[] { });

            return GamesGlobalResultFactory.Success(response);
        }
    }

    public record Response(
        [property: XmlRpcMember("resps")] RespDto[] Resps,
        [property: XmlRpcMember("errors")] ErrorDto[] Errors);

    public record RespDto(
        [property: XmlRpcMember("userInfo")] GamesGlobalUserInfoDto UserInfo,
        [property: XmlRpcMember("cashBal")] long CashBal,
        [property: XmlRpcMember("bonusBal")] long BonusBal,
        [property: XmlRpcMember("currencyBal")] long CurrencyBal,
        [property: XmlRpcMember("currencyBonusBal")] long CurrencyBonusBal,
        [property: XmlRpcMember("gamePlayerRate")] double GamePlayerRate);

    public record ErrorDto;
    // [property: XmlRpcMember("serverId")] int ServerId,
    // [property: XmlRpcMember("betTicket")] string BetTicket,
    // [property: XmlRpcMember("errorCode")] int ErrorCode,
    // [property: XmlRpcMember("requestItemId")] string RequestItemId);
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal.Base.Response;

using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Api.Application.Results.GamesGlobal;

public record GamesGlobalBaseResponse(
    string User,
    GamesGlobalErrorCode Status,
    string RequestUuid,
    string Currency) : BaseResponse
{
    public GamesGlobalBaseResponse(
        string user,
        string currency,
        string requestUuid)
        : this(
            user,
            GamesGlobalErrorCode.CacheError,
            requestUuid,
            currency)
    {
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Base.Response;

using Requests.Base;
using Results.GamesGlobal;

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
            GamesGlobalErrorCode.Configuration,
            requestUuid,
            currency)
    {
    }
}
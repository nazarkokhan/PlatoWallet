namespace Platipus.Wallet.Api.Application.Requests.Wallets.Hub88.Base.Response;

using Requests.Base;
using Results.Hub88;

public record Hub88BaseResponse(
    string User,
    string Status,
    string RequestUuid,
    string Currency) : BaseResponse
{
    public Hub88BaseResponse(
        string user,
        string currency,
        string requestUuid)
        : this(
            user,
            Hub88ErrorCode.RS_OK.ToString(),
            requestUuid,
            currency)
    {
    }
}
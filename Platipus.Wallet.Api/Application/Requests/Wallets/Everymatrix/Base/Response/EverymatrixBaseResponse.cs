namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using Requests.Base;
using Results.Everymatrix;

public record EverymatrixBaseResponse(
    string User,
    EverymatrixErrorCode Status,
    string RequestUuid,
    string Currency) : BaseResponse
{
    public EverymatrixBaseResponse(
        string user,
        string currency,
        string requestUuid)
        : this(
            user,
            EverymatrixErrorCode.InsufficientFunds,
            requestUuid,
            currency)
    {
    }
}
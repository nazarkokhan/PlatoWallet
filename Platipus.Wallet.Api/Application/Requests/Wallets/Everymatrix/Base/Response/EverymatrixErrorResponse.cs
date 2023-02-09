namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using Results.Everymatrix;

public record EverymatrixErrorResponse(
    string Status,
    int ErrorCode,
    string Description,
    string LogId = "") : EverymatrixBaseResponse(Status)
{
    public EverymatrixErrorResponse(EverymatrixErrorCode errorCode)
        : this(
            "Failed",
            (int)errorCode,
            errorCode.ToString())
    {
    }
}
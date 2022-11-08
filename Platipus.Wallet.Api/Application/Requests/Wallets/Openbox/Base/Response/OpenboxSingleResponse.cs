namespace Platipus.Wallet.Api.Application.Requests.Wallets.Openbox.Base.Response;

using Platipus.Wallet.Api.Application.Requests.Base;
using Platipus.Wallet.Api.Application.Results.Openbox;

public record OpenboxSingleResponse(
    bool IsSuccess,
    OpenboxErrorCode ErrorCode,
    string ErrorMsg,
    DateTime Timestamp,
    string Payload) : BaseResponse
{
    public OpenboxSingleResponse(string payload)
        : this(
            true,
            OpenboxErrorCode.Success,
            "",
            DateTime.UtcNow,
            payload)
    {
    }

    public OpenboxSingleResponse(OpenboxErrorCode errorCode)
        : this(
            false,
            errorCode,
            errorCode.ToString(),
            DateTime.UtcNow,
            "")
    {
    }
}
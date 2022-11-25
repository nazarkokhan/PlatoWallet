namespace Platipus.Wallet.Api.Application.Requests.Wallets.Softswiss.Base;

using Requests.Base;

public record SoftswissErrorResponse(
    int Code,
    string? Message,
    long? Balance) : BaseResponse
{
    public SoftswissErrorResponse(SoftswissErrorCode code, long? balance = null)
        : this((int)code, code.ToString(), balance)
    {
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base.Response;

using Results.Uis;

public record UisErrorResponse(
    int Status,
    string Message)
{
    public UisErrorResponse(UisErrorCode errorCode)
        : this((int)errorCode, "FAILED")
    {
    }
}
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Uis.Base.Response;

using Results.Uis;

public record UisBaseResponse(
    int Status,
    string Message)
{
    // Default success response
    protected UisBaseResponse()
        : this(UisErrorCode.InvalidIp, UisErrorCode.ExpiredToken.ToString())
    {
    }

    public UisBaseResponse(UisErrorCode status, string message)
        : this((int)status, message)
    {
    }
}
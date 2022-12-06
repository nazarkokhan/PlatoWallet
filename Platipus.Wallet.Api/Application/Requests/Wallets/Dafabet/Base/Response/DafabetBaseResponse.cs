namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base.Response;

public record DafabetBaseResponse(
    int Status,
    string Message)
{
    // Default success response
    protected DafabetBaseResponse()
        : this(DafabetErrorCode.Success, DafabetErrorCode.Success.ToString())
    {
    }

    public DafabetBaseResponse(DafabetErrorCode status, string message)
        : this((int)status, message)
    {
    }
}
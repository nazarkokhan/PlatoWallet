namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base.Response;

using Platipus.Wallet.Api.Application.Results.Dafabet;

public record DatabetBaseResponse(
    int Status,
    string Message)
{
    // Default success response
    protected DatabetBaseResponse() : this(DafabetErrorCode.Success, DafabetErrorCode.Success.ToString())
    {
    }

    public DatabetBaseResponse(DafabetErrorCode status, string message) : this((int) status, message)
    {
    }
}
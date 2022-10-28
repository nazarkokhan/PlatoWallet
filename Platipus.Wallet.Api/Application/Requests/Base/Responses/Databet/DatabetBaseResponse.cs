namespace Platipus.Wallet.Api.Application.Requests.Base.Responses.Databet;

using Results.Common;

public record DatabetBaseResponse(
    int Status,
    string Message)
{
    // Default success response
    protected DatabetBaseResponse() : this(DatabetErrorCode.Success, DatabetErrorCode.Success.ToString())
    {
    }

    public DatabetBaseResponse(DatabetErrorCode status, string message) : this((int) status, message)
    {
    }
}
namespace PlatipusWallet.Api.Application.Requests.Base.Responses.Databet;

using Results.Common;

public record DatabetBaseResponse(
    DatabetErrorCode Status,
    string Message)
{
    // Default success response
    protected DatabetBaseResponse() : this(DatabetErrorCode.Success, "Success")
    {
    }
}
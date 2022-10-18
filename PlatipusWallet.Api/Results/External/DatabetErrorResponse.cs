namespace PlatipusWallet.Api.Results.External;

using Application.Requests.Base.Responses.Databet;
using Common;

public record DatabetErrorResponse(
    DatabetErrorCode Status,
    string Message) : DatabetBaseResponse(Status, Message);
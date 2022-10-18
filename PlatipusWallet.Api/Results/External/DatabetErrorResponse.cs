namespace PlatipusWallet.Api.Results.External;

using Application.Requests.Base.Responses.Databet;
using Common;

public record DatabetErrorResponse(
    int Status,
    string Message) : DatabetBaseResponse(Status, Message);
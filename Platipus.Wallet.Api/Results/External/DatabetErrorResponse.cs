namespace Platipus.Wallet.Api.Results.External;

using Application.Requests.Base.Responses.Databet;

public record DatabetErrorResponse(
    int Status,
    string Message) : DatabetBaseResponse(Status, Message);
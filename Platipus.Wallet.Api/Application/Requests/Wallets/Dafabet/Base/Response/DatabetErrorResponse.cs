namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base.Response;

public record DatabetErrorResponse(
    int Status,
    string Message) : DatabetBaseResponse(Status, Message);
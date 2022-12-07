namespace Platipus.Wallet.Api.Application.Requests.Wallets.Dafabet.Base.Response;

public record DafabetErrorResponse(
    int Status,
    string Message) : DafabetBaseResponse(Status, Message);
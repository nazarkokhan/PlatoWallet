namespace Platipus.Wallet.Api.Application.Requests.Wallets.Psw.Base.Response;

using Results.Psw;

public record PswErrorResponse(
    Status Status,
    int Error,
    string Description) : PswBaseResponse(Status);
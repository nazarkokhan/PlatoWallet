namespace Platipus.Wallet.Api.Application.Requests.Wallets.BetConstruct.Base.Response;

public record class BetConstructBaseResponse(
    bool Result,
    string? ErrorDescription,
    int? ErrorCode,
    long TransactionId,
    decimal Balance);
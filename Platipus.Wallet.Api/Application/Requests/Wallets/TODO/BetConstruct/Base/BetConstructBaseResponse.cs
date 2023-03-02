namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.BetConstruct.Base;

public record BetConstructBaseResponse(
    bool Result,
    string? ErrDesc,
    int? ErrCode,
    string Transactionid,
    decimal Balance);
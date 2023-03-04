namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base;

public record BetConstructErrorResponse(
    bool Result,
    string ErrDesc,
    int ErrCode);
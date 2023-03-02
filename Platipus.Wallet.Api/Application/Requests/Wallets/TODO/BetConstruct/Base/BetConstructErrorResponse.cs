namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.BetConstruct.Base;

public record BetConstructErrorResponse(
    bool Result,
    string ErrDesc,
    int ErrCode);
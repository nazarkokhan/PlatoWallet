namespace Platipus.Wallet.Api.Application.Responses.Anakatech;

using Base;

public sealed record AnakatechGetPlayerBalanceResponse(
    bool Success,
    long Balance,
    string Currency,
    string? ErrorCode = null,
    double CashBalance = 0,
    double BonusBalance = 0) : AnakatechSuccessResponse(
    Success,
    Balance,
    Currency,
    ErrorCode,
    CashBalance,
    BonusBalance);
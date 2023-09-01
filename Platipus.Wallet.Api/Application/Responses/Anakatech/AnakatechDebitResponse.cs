namespace Platipus.Wallet.Api.Application.Responses.Anakatech;

using Base;

public sealed record AnakatechDebitResponse(
    bool Success,
    string ReferenceId,
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
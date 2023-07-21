namespace Platipus.Wallet.Api.Application.Responses.Anakatech;

using Base;

public sealed record AnakatechCreditResponse(
    bool Success,
    string ReferenceId,
    long Balance,
    string Currency,
    string? ErrorCode = null,
    double CashBalance = 0,
    double BonusBalance = 0) : AnakatechSuccessResponse(
    Success,
    Balance,
    ErrorCode,
    Currency,
    CashBalance,
    BonusBalance);
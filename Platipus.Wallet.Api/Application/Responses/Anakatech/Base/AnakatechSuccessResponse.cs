namespace Platipus.Wallet.Api.Application.Responses.Anakatech.Base;

public record AnakatechSuccessResponse(
    bool Success,
    long Balance,
    string Currency,
    string? ErrorCode = null,
    double CashBalance = 0,
    double BonusBalance = 0) : AnakatechBaseResponse(
    Success,
    Balance,
    ErrorCode,
    CashBalance,
    BonusBalance);
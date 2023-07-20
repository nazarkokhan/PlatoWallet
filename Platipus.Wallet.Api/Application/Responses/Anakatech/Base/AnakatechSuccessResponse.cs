namespace Platipus.Wallet.Api.Application.Responses.Anakatech.Base;

public record AnakatechSuccessResponse(
    bool Success,
    int Balance,
    string? ErrorCode,
    string Currency,
    double CashBalance = 0,
    double BonusBalance = 0) : AnakatechBaseResponse(
    Success,
    Balance,
    ErrorCode,
    CashBalance,
    BonusBalance);
namespace Platipus.Wallet.Api.Application.Responses.Anakatech.Base;

public abstract record AnakatechBaseResponse(
    bool Success,
    long Balance,
    string? ErrorCode,
    double CashBalance = 0,
    double BonusBalance = 0);
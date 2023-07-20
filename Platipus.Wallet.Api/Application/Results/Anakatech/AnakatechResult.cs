namespace Platipus.Wallet.Api.Application.Results.Anakatech;

using Base;
using Humanizer;

public sealed record AnakatechResult : BaseResult<AnakatechErrorCode>, IAnakatechResult
{
    public AnakatechResult()
    {
        Balance = default;
        ErrorCode = AnakatechErrorCode.InternalError.Humanize();
        Success = false;
        CashBalance = 0;
        BonusBalance = 0;
    }
    
    public AnakatechResult(
        AnakatechErrorCode errorCode,
        int balance,
        bool success,
        Exception? exception = null)
        : base(errorCode, exception)
    {
        Balance = balance;
        ErrorCode = errorCode.Humanize();
        Success = success;
        CashBalance = 0;
        BonusBalance = 0;
    }
    public bool Success { get; }
    public int Balance { get; }
    public double CashBalance { get; }
    public double BonusBalance { get; }
    public string? ErrorCode { get; }
}
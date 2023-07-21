namespace Platipus.Wallet.Api.Application.Results.Anakatech.WithData;

using Base.WithData;
using Humanizer;

public sealed record AnakatechResult<TData> : BaseResult<AnakatechErrorCode, TData>, IAnakatechResult<TData>
{
    public AnakatechResult(TData data)
        : base(data)
    {
    }

    public AnakatechResult(
        AnakatechErrorCode errorCode,
        long balance,
        bool success,
        Exception? exception)
        : base(errorCode, exception)
    {
        CashBalance = 0;
        BonusBalance = 0;
        Success = success;
        Balance = balance;
        ErrorCode = errorCode.Humanize();
    }

    public bool Success { get; }
    public long Balance { get; }
    public double CashBalance { get; }
    public double BonusBalance { get; }
    public string? ErrorCode { get; }
}
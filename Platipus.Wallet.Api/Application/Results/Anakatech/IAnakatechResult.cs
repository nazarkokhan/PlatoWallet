namespace Platipus.Wallet.Api.Application.Results.Anakatech;

using Base;

public interface IAnakatechResult : IBaseResult<AnakatechErrorCode>
{
    public bool Success { get; }

    public long Balance { get; }

    public double CashBalance { get; }

    public double BonusBalance { get; }

    public string? ErrorCode { get; }
}
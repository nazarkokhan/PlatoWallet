namespace Platipus.Wallet.Api.Application.Results.Evenbet.WithData;

using Base.WithData;

public sealed record EvenbetResult<TData> : BaseResult<EvenbetErrorCode, TData>, IEvenbetResult<TData>
{
    public EvenbetResult(TData data)
        : base(data)
    {
    }

    public EvenbetResult(
        EvenbetErrorCode errorCode,
        int balance,
        string timestamp,
        Exception? exception = null)
        : base(errorCode, exception)
    {
        Balance = balance;
        Timestamp = timestamp;
    }

    public int Balance { get; }
    public string Timestamp { get; }
}
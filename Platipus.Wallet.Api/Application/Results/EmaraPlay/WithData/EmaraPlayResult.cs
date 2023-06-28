namespace Platipus.Wallet.Api.Application.Results.EmaraPlay.WithData;

using Base.WithData;

public record EmaraPlayResult<TData> 
    : BaseResult<EmaraPlayErrorCode, TData>, IEmaraPlayResult<TData>
{
    public EmaraPlayResult(TData data)
        : base(data)
    {
    }

    public EmaraPlayResult(
        EmaraPlayErrorCode errorCode,
        long? balance = null,
        Exception? exception = null)
        : base(errorCode, exception)
    {
        Balance = balance;
    }

    public long? Balance { get; }
}
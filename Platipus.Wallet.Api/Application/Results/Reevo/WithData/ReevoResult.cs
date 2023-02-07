namespace Platipus.Wallet.Api.Application.Results.Reevo.WithData;

using Base.WithData;

public record ReevoResult<TData> : BaseResult<ReevoErrorCode, TData>, IReevoResult<TData>
{
    public ReevoResult(TData data)
        : base(data)
    {
    }

    public ReevoResult(
        ReevoErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}
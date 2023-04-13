namespace Platipus.Wallet.Api.Application.Results.Base.WithData;

public record BaseResult<TError, TData> : BaseResult<TError>, IBaseResult<TError, TData>
{
    public BaseResult(TData data)
    {
        Data = data;
    }

    public BaseResult(
        TError errorCode,
        Exception? exception)
        : base(errorCode, exception)
    {
        Data = default!;
    }

    public TData Data { get; }
}
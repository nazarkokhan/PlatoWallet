namespace Platipus.Wallet.Api.Application.Results.Base.WithData;

public record BaseResult<TError, TData> : BaseResult<TError>, IBaseResult<TError, TData>
{
    public BaseResult(TData data)
    {
        Data = data;
    }

    public BaseResult(
        TError errorCode,
        Exception? exception = null) : base(errorCode, exception)
    {
        Data = default!;
    }

    public TData Data { get; }

    public IBaseResult<TNewError, TNewData> ConvertResult<TNewError, TNewData>(TNewError error, TNewData data)
    {
        return new BaseResult<TNewError, TNewData>(error, Exception);
    }
}
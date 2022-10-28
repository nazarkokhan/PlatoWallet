namespace Platipus.Wallet.Api.Results.Common.Result.WithData;

public record Result<TData> : BaseResult<ErrorCode, TData>, IResult<TData>
{
    public Result(TData data) : base(data)
    {
    }

    public Result(
        ErrorCode errorCode,
        Exception? exception = null) : base(errorCode, exception)
    {
    }
}

public record DatabetResult<TData> : BaseResult<DatabetErrorCode, TData>, IDatabetResult<TData>
{
    public DatabetResult(TData data) : base(data)
    {
    }

    public DatabetResult(
        DatabetErrorCode errorCode,
        Exception? exception = null) : base(errorCode, exception)
    {
    }
}

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
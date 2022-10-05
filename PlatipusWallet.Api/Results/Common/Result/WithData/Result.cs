namespace PlatipusWallet.Api.Results.Common.Result.WithData;

public record Result<T> : Result, IResult<T>
{
    public Result(T data)
    {
        Data = data;
    }

    public Result(
        ErrorCode errorCode,
        Exception? exception = null,
        string? description = null) : base(errorCode, exception, description)
    {
        Data = default!;
    }

    public T Data { get; }
}
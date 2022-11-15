namespace Platipus.Wallet.Api.Application.Results.Common.WithData;

using Base.WithData;

public record Result<TData> : BaseResult<ErrorCode, TData>, IResult<TData>
{
    public Result(TData data)
        : base(data)
    {
    }

    public Result(
        ErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}
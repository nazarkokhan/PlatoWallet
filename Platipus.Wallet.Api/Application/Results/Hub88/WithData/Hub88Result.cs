namespace Platipus.Wallet.Api.Application.Results.Hub88.WithData;

using Base.WithData;

public record Hub88Result<TData> : BaseResult<Hub88ErrorCode, TData>, IHub88Result<TData>
{
    public Hub88Result(TData data)
        : base(data)
    {
    }

    public Hub88Result(
        Hub88ErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}
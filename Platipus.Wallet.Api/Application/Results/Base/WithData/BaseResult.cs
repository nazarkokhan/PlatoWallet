namespace Platipus.Wallet.Api.Application.Results.Base.WithData;

using System.Text.Json.Serialization;

public record BaseResult<TError, TData> : BaseResult<TError>, IBaseResult<TError, TData>
{
    private readonly TData _data;

    public BaseResult(TData data)
    {
        _data = data;
    }

    public BaseResult(
        TError errorCode,
        Exception? exception)
        : base(errorCode, exception)
    {
        _data = default!;
    }

    public TData Data => IsSuccess ? _data : throw new Exception("Data is empty because result is failed");
}
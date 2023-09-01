namespace Platipus.Wallet.Api.Application.Results.Parimatch.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public record ParimatchResult<TData> : BaseResult<ParimatchErrorCode, TData>, IParimatchResult<TData>
{
    public ParimatchResult(TData data)
        : base(data)
    {
    }

    public ParimatchResult(
        ParimatchErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}
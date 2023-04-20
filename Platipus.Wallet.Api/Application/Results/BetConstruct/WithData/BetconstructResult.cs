namespace Platipus.Wallet.Api.Application.Results.BetConstruct.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public record BetconstructResult<TData> : BaseResult<BetconstructErrorCode, TData>, IBetconstructResult<TData>
{
    public BetconstructResult(TData data)
        : base(data)
    {
    }

    public BetconstructResult(
        BetconstructErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}
namespace Platipus.Wallet.Api.Application.Results.PariMatch.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;
using PariMatch;

public record PariMatchResult<TData> : BaseResult<PariMatchErrorCode, TData>, IPariMatchResult<TData>
{
    public PariMatchResult(TData data)
        : base(data)
    {
    }

    public PariMatchResult(
        PariMatchErrorCode errorCode,
        Exception? exception = null)
        : base(errorCode, exception)
    {
    }
}
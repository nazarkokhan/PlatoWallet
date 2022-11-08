namespace Platipus.Wallet.Api.Application.Results.Dafabet.WithData;

using Base.WithData;

public record DafabetResult<TData> : BaseResult<DafabetErrorCode, TData>, IDafabetResult<TData>
{
    public DafabetResult(TData data) : base(data)
    {
    }

    public DafabetResult(
        DafabetErrorCode errorCode,
        Exception? exception = null) : base(errorCode, exception)
    {
    }
}
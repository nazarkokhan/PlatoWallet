namespace Platipus.Wallet.Api.Application.Results.Dafabet.WithData;

using Base.WithData;

public interface IDafabetResult<out TData> : IBaseResult<DafabetErrorCode, TData>, IDafabetResult
{
}
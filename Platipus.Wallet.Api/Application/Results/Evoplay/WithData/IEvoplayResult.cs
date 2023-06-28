namespace Platipus.Wallet.Api.Application.Results.Evoplay.WithData;

using Base.WithData;

public interface IEvoplayResult<out TData> : IBaseResult<EvoplayErrorCode, TData>, IEvoplayResult
{
    
}
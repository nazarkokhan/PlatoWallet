using Platipus.Wallet.Api.Application.Results.Base.WithData;

namespace Platipus.Wallet.Api.Application.Results.Sweepium.WithData;
public interface ISweepiumResult<out TData> : IBaseResult<SweepiumErrorCode, TData>, ISweepiumResult
{
    
}
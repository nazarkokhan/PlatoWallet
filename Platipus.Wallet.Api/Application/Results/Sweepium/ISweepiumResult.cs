using Platipus.Wallet.Api.Application.Results.Base.WithData;
using Platipus.Wallet.Api.Application.Results.Sweepium.WithData;

namespace Platipus.Wallet.Api.Application.Results.Sweepium;

public interface ISweepiumResult<out TData> : IBaseResult<SweepiumErrorCode, TData>, ISweepiumResult
{
    
}
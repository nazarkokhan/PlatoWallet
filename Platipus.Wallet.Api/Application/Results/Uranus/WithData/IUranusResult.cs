namespace Platipus.Wallet.Api.Application.Results.Uranus.WithData;

using Platipus.Wallet.Api.Application.Results.Base.WithData;

public interface IUranusResult<out TData> : IBaseResult<UranusErrorCode, TData>, IUranusResult
{
    
}
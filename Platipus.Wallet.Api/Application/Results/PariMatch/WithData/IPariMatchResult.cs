namespace Platipus.Wallet.Api.Application.Results.Parimatch.WithData;

using Base.WithData;

public interface IParimatchResult<out TData> : IBaseResult<ParimatchErrorCode, TData>, IParimatchResult
{
}
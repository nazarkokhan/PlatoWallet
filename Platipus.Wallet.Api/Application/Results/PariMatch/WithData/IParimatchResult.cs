namespace Platipus.Wallet.Api.Application.Results.PariMatch.WithData;

using Base.WithData;

public interface IParimatchResult<out TData> : IBaseResult<ParimatchErrorCode, TData>, IParimatchResult
{
}
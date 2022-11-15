namespace Platipus.Wallet.Api.Application.Results.Hub88.WithData;

using Base.WithData;

public interface IHub88Result<out TData> : IBaseResult<Hub88ErrorCode, TData>, IHub88Result
{
}
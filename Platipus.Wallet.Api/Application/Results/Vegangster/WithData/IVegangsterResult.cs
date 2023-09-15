namespace Platipus.Wallet.Api.Application.Results.Vegangster.WithData;

using Base.WithData;

public interface IVegangsterResult<out TData> : IBaseResult<VegangsterResponseStatus, TData>, IVegangsterResult
{
}
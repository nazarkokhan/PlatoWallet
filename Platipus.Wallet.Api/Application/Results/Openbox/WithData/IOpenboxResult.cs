namespace Platipus.Wallet.Api.Application.Results.Openbox.WithData;

using Base.WithData;

public interface IOpenboxResult<out TData> : IBaseResult<OpenboxErrorCode, TData>, IOpenboxResult
{
}
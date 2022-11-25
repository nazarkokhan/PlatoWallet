namespace Platipus.Wallet.Api.Application.Results.Softswiss.WithData;

using Base.WithData;

public interface ISoftswissResult<out TData> : IBaseResult<SoftswissErrorCode, TData>, ISoftswissResult
{
}
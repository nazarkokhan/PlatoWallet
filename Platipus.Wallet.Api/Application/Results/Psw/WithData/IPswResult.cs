namespace Platipus.Wallet.Api.Application.Results.Psw.WithData;

using Base.WithData;

public interface IPswResult<out TData> : IBaseResult<PswErrorCode, TData>, IPswResult
{
}
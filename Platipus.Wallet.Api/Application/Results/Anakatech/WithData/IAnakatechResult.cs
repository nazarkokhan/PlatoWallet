namespace Platipus.Wallet.Api.Application.Results.Anakatech.WithData;

using Base.WithData;

public interface IAnakatechResult<out TData> : IBaseResult<AnakatechErrorCode, TData>, IAnakatechResult
{
}
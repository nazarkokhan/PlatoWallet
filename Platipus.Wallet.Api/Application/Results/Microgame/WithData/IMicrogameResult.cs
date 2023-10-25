namespace Platipus.Wallet.Api.Application.Results.Microgame.WithData;

using Base.WithData;

public interface IMicrogameResult<out TData> : IBaseResult<MicrogameStatusCode, TData>, IMicrogameResult
{
}
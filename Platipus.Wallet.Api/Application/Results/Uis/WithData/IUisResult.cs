namespace Platipus.Wallet.Api.Application.Results.Uis.WithData;

using Base.WithData;

public interface IUisResult<out TData> : IBaseResult<UisErrorCode, TData>, IUisResult
{
}
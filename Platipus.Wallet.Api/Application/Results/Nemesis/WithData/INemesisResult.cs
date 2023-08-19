namespace Platipus.Wallet.Api.Application.Results.Nemesis.WithData;

using Base.WithData;

public interface INemesisResult<out TData> : IBaseResult<NemesisErrorCode, TData>, INemesisResult
{
}
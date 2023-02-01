namespace Platipus.Wallet.Api.Application.Results.PariMatch.WithData;

using Base.WithData;

public interface IPariMatchResult<out TData> : IBaseResult<PariMatchErrorCode, TData>, IPariMatchResult
{
}
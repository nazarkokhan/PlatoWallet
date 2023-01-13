namespace Platipus.Wallet.Api.Application.Results.PariMatch.WithData;

using Hub88;
using Platipus.Wallet.Api.Application.Results.Base.WithData;
using PariMatch;

public interface IPariMatchResult<out TData> : IBaseResult<PariMatchErrorCode, TData>, IPariMatchResult
{
}
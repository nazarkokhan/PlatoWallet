namespace Platipus.Wallet.Api.Application.Results.Common.WithData;

using Base.WithData;

public interface IResult<out TData> : IBaseResult<ErrorCode, TData>, IResult
{
}
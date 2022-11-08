namespace Platipus.Wallet.Api.Application.Results.Psw.WithData;

using Base.WithData;

public interface IResult<out TData> : IBaseResult<ErrorCode, TData>, IResult
{
}
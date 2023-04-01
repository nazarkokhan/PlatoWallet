namespace Platipus.Wallet.Api.Application.Results.Base;

public interface IBaseResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    Exception? Exception { get; }

    IBaseResult<TNewError> ConvertResult<TNewError>(TNewError error);
}

public interface IBaseResult<out TError> : IBaseResult
{
    TError ErrorCode { get; }
}
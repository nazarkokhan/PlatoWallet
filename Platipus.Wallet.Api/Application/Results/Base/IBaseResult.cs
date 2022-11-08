namespace Platipus.Wallet.Api.Application.Results.Base;

public interface IBaseResult<out TError>
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    TError ErrorCode { get; }

    Exception? Exception { get; }

    IBaseResult<TNewError> ConvertResult<TNewError>(TNewError error);
}
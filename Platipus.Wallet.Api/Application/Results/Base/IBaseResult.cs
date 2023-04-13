namespace Platipus.Wallet.Api.Application.Results.Base;

public interface IBaseResult
{
    bool IsSuccess { get; }

    bool IsFailure { get; }

    Exception? Exception { get; }
}

public interface IBaseResult<out TError> : IBaseResult
{
    TError Error { get; }
}
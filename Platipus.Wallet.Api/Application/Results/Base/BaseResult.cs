namespace Platipus.Wallet.Api.Application.Results.Base;

public record BaseResult<TError> : IBaseResult<TError>
{
    public BaseResult()
    {
        IsSuccess = true;
        Error = default!;
        Exception = null;
    }

    public BaseResult(TError errorCode, Exception? exception = null)
    {
        IsSuccess = false;
        Error = errorCode;
        Exception = exception;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public TError Error { get; }

    public Exception? Exception { get; }
}
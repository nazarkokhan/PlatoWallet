namespace Platipus.Wallet.Api.Application.Results.Base;

public record BaseResult<TError> : IBaseResult<TError>
{
    public BaseResult()
    {
        IsSuccess = true;
        ErrorCode = default!;
        Exception = null;
    }

    public BaseResult(TError errorCode, Exception? exception = null)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        Exception = exception;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public TError ErrorCode { get; }

    public Exception? Exception { get; }
    
    public IBaseResult<T> ConvertResult<T>(T error)
    {
        return new BaseResult<T>(error, Exception);
    }
    
    public IBaseResult<T> ConvertResult<T>()
    {
        return new BaseResult<T>();
    }
}
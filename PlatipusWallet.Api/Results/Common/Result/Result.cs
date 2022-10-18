namespace PlatipusWallet.Api.Results.Common.Result;

using System;

public record Result : BaseResult<ErrorCode>, IResult
{
    public Result()
    {
        ErrorDescription = string.Empty;
    }

    public Result(
        ErrorCode errorCode,
        Exception? exception = null,
        string? description = null) : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}

public record DatabetResult : BaseResult<DatabetErrorCode>, IDatabetResult
{
    public DatabetResult()
    {
    }

    public DatabetResult(
        DatabetErrorCode errorCode,
        Exception? exception = null,
        string? description = null) : base(errorCode, exception)
    {
    }
}

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
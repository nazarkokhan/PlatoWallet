namespace PlatipusWallet.Api.Results.Common.Result;

using System;

public record Result : IResult
{
    public Result()
    {
        IsSuccess = true;
        ErrorCode = ErrorCode.Unknown;
        Exception = null;
        ErrorDescription = string.Empty;
    }

    public Result(ErrorCode errorCode, string? description = null) : this(errorCode, null, description)
    {
        
    }
    public Result(ErrorCode errorCode, Exception? exception = null, string? description = null)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        ErrorDescription = description ?? string.Empty;
        Exception = exception;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ErrorCode ErrorCode { get; }
    
    public string ErrorDescription { get; set; }

    public Exception? Exception { get; }
}
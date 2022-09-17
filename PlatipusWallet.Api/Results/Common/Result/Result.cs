namespace PlatipusWallet.Api.Results.Common.Result;

using System;

public record Result : IResult
{
    public Result()
    {
        IsSuccess = true;
        ErrorCode = ErrorCode.Unknown;
        Exception = null;
    } 

    public Result(ErrorCode errorCode, Exception? exception = null)
    {
        IsSuccess = false;
        ErrorCode = errorCode;
        Exception = exception;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public ErrorCode ErrorCode { get; }

    public Exception? Exception { get; }
}
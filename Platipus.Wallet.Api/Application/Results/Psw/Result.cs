namespace Platipus.Wallet.Api.Application.Results.Psw;

using System;
using Base;

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
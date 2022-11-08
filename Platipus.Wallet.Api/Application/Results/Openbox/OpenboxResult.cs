namespace Platipus.Wallet.Api.Application.Results.Openbox;

using System;
using Base;

public record OpenboxResult : BaseResult<OpenboxErrorCode>, IOpenboxResult
{
    public OpenboxResult()
    {
        ErrorDescription = string.Empty;
    }

    public OpenboxResult(
        OpenboxErrorCode errorCode,
        Exception? exception = null,
        string? description = null) : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}
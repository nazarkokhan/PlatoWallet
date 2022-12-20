namespace Platipus.Wallet.Api.Application.Results.Uis;

using Base;

public record UisResult : BaseResult<UisErrorCode>, IUisResult
{
    public UisResult()
    {
        ErrorDescription = string.Empty;
    }

    public UisResult(
        UisErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}
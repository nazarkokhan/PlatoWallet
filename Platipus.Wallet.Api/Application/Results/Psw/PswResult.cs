namespace Platipus.Wallet.Api.Application.Results.Psw;

using Base;

public record PswResult : BaseResult<PswErrorCode>, IPswResult
{
    public PswResult()
    {
        ErrorDescription = string.Empty;
    }

    public PswResult(
        PswErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}
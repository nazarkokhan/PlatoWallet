namespace Platipus.Wallet.Api.Application.Results.Nemesis;

using Base;

public record NemesisResult : BaseResult<NemesisErrorCode>, INemesisResult
{
    public NemesisResult()
    {
        ErrorDescription = string.Empty;
    }

    public NemesisResult(
        NemesisErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}
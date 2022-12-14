namespace Platipus.Wallet.Api.Application.Results.GamesGlobal;

using Base;

public record GamesGlobalResult : BaseResult<GamesGlobalErrorCode>, IGamesGlobalResult
{
    public GamesGlobalResult()
    {
        ErrorDescription = string.Empty;
    }

    public GamesGlobalResult(
        GamesGlobalErrorCode errorCode,
        Exception? exception = null,
        string? description = null)
        : base(errorCode, exception)
    {
        ErrorDescription = description ?? string.Empty;
    }

    public string ErrorDescription { get; set; }
}
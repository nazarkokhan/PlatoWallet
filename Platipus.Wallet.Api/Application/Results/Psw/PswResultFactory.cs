namespace Platipus.Wallet.Api.Application.Results.Psw;

public static class PswResultFactory
{
    public static PswResult Success()
        => new();

    public static PswResult<TData> Success<TData>(TData data)
        => new(data);

    public static PswResult Failure(PswErrorCode errorCode, Exception? exception = null)
        => new(errorCode, exception);

    public static PswResult<TData> Failure<TData>(
        PswErrorCode errorCode,
        Exception? exception = null)
        => new(errorCode, exception);

    public static PswResult<TData> Failure<TData, TSourceData>(IPswResult<TSourceData> pswResult)
        => pswResult.IsFailure
            ? Failure<TData>(pswResult.Error, pswResult.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(pswResult));
}
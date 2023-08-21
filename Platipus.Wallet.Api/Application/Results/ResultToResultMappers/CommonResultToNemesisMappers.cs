namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Nemesis;
using Nemesis.WithData;

public static class CommonResultToNemesisMappers
{
    public static INemesisResult<TData> ToNemesisFailureResult<TData>(this IResult result)
    {
        return result.IsFailure
            ? NemesisResultFactory.Failure<TData>(result.Error.ToNemesisErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));
    }

    private static NemesisErrorCode ToNemesisErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.BadParametersInTheRequest => NemesisErrorCode.InappropriateArgument,
            ErrorCode.SessionExpired => NemesisErrorCode.SessionExpired,
            ErrorCode.SessionNotFound => NemesisErrorCode.SessionNotFound,
            ErrorCode.UserNotFound => NemesisErrorCode.UserNotFound,
            ErrorCode.GameNotFound => NemesisErrorCode.GameNotFound,
            ErrorCode.CasinoNotFound => NemesisErrorCode.ProviderNotFound,
            ErrorCode.InsufficientFunds => NemesisErrorCode.InsufficientFunds,
            ErrorCode.RoundAlreadyFinished => NemesisErrorCode.RoundIsClosed,
            ErrorCode.InvalidCurrency => NemesisErrorCode.CurrencyMismatched,
            // ReSharper disable once PatternIsRedundant
            ErrorCode.Unknown or _ => NemesisErrorCode.Internal
        };
    }
}
namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Dafabet;
using Dafabet.WithData;

public static class CommonResultToDafabetMappers
{
    public static IDafabetResult<TData> ToDafabetResult<TData>(this IResult result)
        => result.IsFailure
            ? DafabetResultFactory.Failure<TData>(result.Error.ToDafabetErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IDafabetResult ToDafabetResult(this IResult result)
        => result.IsSuccess
            ? DafabetResultFactory.Success()
            : DafabetResultFactory.Failure(result.Error.ToDafabetErrorCode(), result.Exception);

    private static DafabetErrorCode ToDafabetErrorCode(this ErrorCode source)
    {
        return source switch
        {
            // ErrorCode.Success=> DafabetErrorCode.Success,
            ErrorCode.SecurityParameterIsInvalid or ErrorCode.SecurityParameterIsEmpty => DafabetErrorCode.InvalidHash,
            ErrorCode.BadParametersInTheRequest => DafabetErrorCode.MissingRequiredParameters,
            ErrorCode.SessionExpired or ErrorCode.SessionNotFound => DafabetErrorCode.InvalidToken,
            ErrorCode.GameNotFound => DafabetErrorCode.InvalidGameCode,
            ErrorCode.UserNotFound or ErrorCode.UserIsDisabled => DafabetErrorCode.PlayerNotFound,
            ErrorCode.RoundNotFound => DafabetErrorCode.RoundNotFound,
            ErrorCode.TransactionNotFound => DafabetErrorCode.TransactionNotFound,
            ErrorCode.InsufficientFunds => DafabetErrorCode.InsufficientFunds,
            ErrorCode.Unknown or _ => DafabetErrorCode.SystemError
        };
    }
}
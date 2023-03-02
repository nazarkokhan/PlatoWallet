namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using GamesGlobal;
using GamesGlobal.WithData;

public static class CommonResultToGamesGlobalMappers
{
    public static IGamesGlobalResult<TData> ToGamesGlobalResult<TData>(this IResult result)
        => result.IsFailure
            ? GamesGlobalResultFactory.Failure<TData>(result.ErrorCode.ToGamesGlobalErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IGamesGlobalResult ToGamesGlobalResult(this IResult result)
        => result.IsSuccess
            ? GamesGlobalResultFactory.Success()
            : GamesGlobalResultFactory.Failure(result.ErrorCode.ToGamesGlobalErrorCode(), result.Exception);

    private static GamesGlobalErrorCode ToGamesGlobalErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.InsufficientFunds => GamesGlobalErrorCode.InvalidTransactionAmount,
            ErrorCode.UserIsDisabled => GamesGlobalErrorCode.PlayerAccountClosed,
            ErrorCode.SessionExpired => GamesGlobalErrorCode.PlayerIsNotLoggedInOrSessionHasExpired,
            ErrorCode.SecurityParameterIsEmpty or ErrorCode.SecurityParameterIsInvalid => GamesGlobalErrorCode.InvalidAPICredentials,
            ErrorCode.BadParametersInTheRequest => GamesGlobalErrorCode.FreeGameInvalidParameters,
            ErrorCode.CasinoNotFound => GamesGlobalErrorCode.MissingServerConfiguration,
            ErrorCode.GameNotFound => GamesGlobalErrorCode.InvalidGameName,
            ErrorCode.InvalidCurrency => GamesGlobalErrorCode.IncorrectCurrency,
            ErrorCode.TransactionAlreadyExists => GamesGlobalErrorCode.UnresolvedTicketsOnCompleteGame,
            ErrorCode.TransactionNotFound => GamesGlobalErrorCode.InvalidTicketId,
            ErrorCode.Unknown or _ => GamesGlobalErrorCode.UnknownServerError
        };
    }
}
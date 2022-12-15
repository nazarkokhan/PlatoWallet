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

    public static GamesGlobalErrorCode ToGamesGlobalErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => GamesGlobalErrorCode.InvalidTransactionAmount,
            ErrorCode.UserDisabled => GamesGlobalErrorCode.PlayerAccountClosed,
            ErrorCode.SessionExpired => GamesGlobalErrorCode.PlayerIsNotLoggedInOrSessionHasExpired,
            ErrorCode.MissingSignature or ErrorCode.InvalidSignature => GamesGlobalErrorCode.InvalidAPICredentials,
            ErrorCode.BadParametersInTheRequest => GamesGlobalErrorCode.FreeGameInvalidParameters,
            ErrorCode.InvalidCasinoId => GamesGlobalErrorCode.MissingServerConfiguration,
            ErrorCode.InvalidGame => GamesGlobalErrorCode.InvalidGameName,
            ErrorCode.WrongCurrency => GamesGlobalErrorCode.IncorrectCurrency,
            ErrorCode.DuplicateTransaction => GamesGlobalErrorCode.UnresolvedTicketsOnCompleteGame,
            ErrorCode.TransactionDoesNotExist => GamesGlobalErrorCode.InvalidTicketId,
            ErrorCode.Unknown or _ => GamesGlobalErrorCode.UnknownServerError
        };
    }
}
namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Atlas;
using Atlas.WithData;

public static class CommonResultToAtlasMappers
{
    public static IAtlasResult<TData> ToAtlasFailureResult<TData>(this IResult result)
        => result.IsFailure
            ? AtlasResultFactory.Failure<TData>(result.Error.ToAtlasErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IAtlasResult ToAtlasResult(this IResult result)
        => result.IsSuccess
            ? AtlasResultFactory.Success()
            : AtlasResultFactory.Failure(
                result.Error.ToAtlasErrorCode(),
                exception: result.Exception);

    public static IAtlasResult<TData> ToAtlasResult<TData>(this IResult result, TData response)
        => result.IsSuccess
            ? AtlasResultFactory.Success(response)
            : AtlasResultFactory.Failure<TData>(
                result.Error.ToAtlasErrorCode(),
                exception: result.Exception);

    private static AtlasErrorCode ToAtlasErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.CasinoNotFound => AtlasErrorCode.ProviderNotConfigured,
            ErrorCode.TransactionAlreadyExists => AtlasErrorCode.TransactionAlreadyProcessed,
            ErrorCode.TransactionNotFound => AtlasErrorCode.BetTransactionNotFound,
            ErrorCode.UserNotFound => AtlasErrorCode.SessionValidationFailed,
            ErrorCode.UserIsDisabled => AtlasErrorCode.SessionValidationFailed,
            ErrorCode.RoundAlreadyExists => AtlasErrorCode.GameRoundNotPreviouslyCreated,
            ErrorCode.RoundAlreadyFinished => AtlasErrorCode.GameRoundIdNotUnique,
            ErrorCode.InvalidCurrency => AtlasErrorCode.CurrencyMismatchException,
            ErrorCode.GameServerApiError => AtlasErrorCode.GameLaunchError,
            ErrorCode.RoundNotFound => AtlasErrorCode.GameRoundNotPreviouslyCreated,
            ErrorCode.InsufficientFunds => AtlasErrorCode.InsufficientFunds,
            ErrorCode.UnknownHttpClientError => AtlasErrorCode.InternalError,
            _ => AtlasErrorCode.InternalError
        };
    }
}
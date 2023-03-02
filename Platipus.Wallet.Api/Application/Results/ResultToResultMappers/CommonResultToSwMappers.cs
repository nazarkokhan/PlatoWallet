namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Sw;
using Sw.WithData;

public static class CommonResultToSwMappers
{
    public static ISwResult<TData> ToSwResult<TData>(this IResult result)
        => result.IsFailure
            ? SwResultFactory.Failure<TData>(result.ErrorCode.ToSwErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static ISwResult ToSwResult(this IResult result)
        => result.IsSuccess
            ? SwResultFactory.Success()
            : SwResultFactory.Failure(result.ErrorCode.ToSwErrorCode(), result.Exception);

    private static SwErrorCode ToSwErrorCode(this ErrorCode source)
    {
        return source switch
        {
            // ErrorCode.InsufficientFunds => SwErrorCode.InvalidAmount,
            // ErrorCode.UserIsDisabled => SwErrorCode.UserNotFound,
            // ErrorCode.SessionExpired => SwErrorCode.ExpiredToken,
            // ErrorCode.SecurityParameterIsEmpty or ErrorCode.SecurityParameterIsInvalid or ErrorCode.SecurityParameterIsInvalid => SwErrorCode.InvalidMd5OrHash,
            // ErrorCode.CasinoNotFound => SwErrorCode.InvalidPartnered,
            // ErrorCode.TransactionAlreadyExists => SwErrorCode.TransactionAlreadyProcessed,
            // ErrorCode.TransactionNotFound => SwErrorCode.InvalidTransactionId,
            // ErrorCode.Unknown or _ => SwErrorCode.InternalSystemError
            ErrorCode.UserNotFound => SwErrorCode.UserNotFound,
            ErrorCode.CasinoNotFound => SwErrorCode.InvalidPartnered,
            ErrorCode.SecurityParameterIsInvalid => SwErrorCode.InvalidMd5OrHash,
            // ErrorCode. => SwErrorCode.InvalidIp,
            // ErrorCode. => SwErrorCode.InvalidAmount,
            ErrorCode.InsufficientFunds => SwErrorCode.InsufficientBalance,
            // ErrorCode. => SwErrorCode.TransferLimit,
            // ErrorCode. => SwErrorCode.DuplicateRemoteTransactionId,
            // ErrorCode. => SwErrorCode.InsufficientBalance2,
            ErrorCode.TransactionNotFound => SwErrorCode.InvalidTransactionId,
            ErrorCode.TransactionAlreadyExists => SwErrorCode.TransactionAlreadyProcessed,
            ErrorCode.SessionExpired => SwErrorCode.ExpiredToken,
            ErrorCode.Unknown or _ => SwErrorCode.InternalSystemError
        };
    }
}
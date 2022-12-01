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

    public static SwErrorCode ToSwErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => SwErrorCode.InvalidAmount,
            ErrorCode.UserDisabled => SwErrorCode.UserNotFound,
            ErrorCode.SessionExpired => SwErrorCode.ExpiredToken,
            ErrorCode.MissingSignature or ErrorCode.InvalidSignature or ErrorCode.InvalidSign => SwErrorCode.InvalidMd5OrHash,
            ErrorCode.InvalidCasinoId => SwErrorCode.InvalidPartnered,
            ErrorCode.DuplicateTransaction => SwErrorCode.TransactionAlreadyProcessed,
            ErrorCode.TransactionDoesNotExist => SwErrorCode.InvalidTransactionId,
            ErrorCode.Unknown or _ => SwErrorCode.InternalSystemError
        };
    }
}
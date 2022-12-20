namespace Platipus.Wallet.Api.Application.Results.ResultToResultMappers;

using Uis;
using Uis.WithData;

public static class CommonResultToUisMappers
{
    public static IUisResult<TData> ToUisResult<TData>(this IResult result)
        => result.IsFailure
            ? UisResultFactory.Failure<TData>(result.ErrorCode.ToUisErrorCode(), result.Exception)
            : throw new ArgumentException("Can not create failure result from success result", nameof(result));

    public static IUisResult ToUisResult(this IResult result)
        => result.IsSuccess
            ? UisResultFactory.Success()
            : UisResultFactory.Failure(result.ErrorCode.ToUisErrorCode(), result.Exception);

    public static UisErrorCode ToUisErrorCode(this ErrorCode source)
    {
        return source switch
        {
            ErrorCode.NotEnoughMoney => UisErrorCode.InvalidAmount,
            ErrorCode.UserDisabled => UisErrorCode.UserNotFound,
            ErrorCode.SessionExpired => UisErrorCode.ExpiredToken,
            ErrorCode.MissingSignature or ErrorCode.InvalidSignature or ErrorCode.InvalidSign => UisErrorCode.InvalidMd5OrHash,
            ErrorCode.InvalidCasinoId => UisErrorCode.InvalidPartnered,
            ErrorCode.DuplicateTransaction => UisErrorCode.TransactionAlreadyProcessed,
            ErrorCode.TransactionDoesNotExist => UisErrorCode.InvalidTransactionId,
            ErrorCode.Unknown or _ => UisErrorCode.InternalSystemError
        };
    }
}
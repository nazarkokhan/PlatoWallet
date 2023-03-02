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

    private static UisErrorCode ToUisErrorCode(this ErrorCode source)
    {
        return source switch
        {
             // UIS system to CP system API Error Calls
             ErrorCode.InsufficientFunds => UisErrorCode.InsufficientFunds,
             ErrorCode.SecurityParameterIsInvalid => UisErrorCode.InvalidHash,
             ErrorCode.TransactionNotFound or ErrorCode.TransactionAlreadyExists =>
                 UisErrorCode.UnknownTransactionIdOrWasAlreadyProcessed,
             ErrorCode.UserNotFound => UisErrorCode.UnknownUserId,
             ErrorCode.UnknownBetException or ErrorCode.UnknownWinException => UisErrorCode.OperationFailed,
             ErrorCode.SessionNotFound => UisErrorCode.InvalidToken,
             ErrorCode.Unknown or _ => UisErrorCode.InternalError,

            // CP system to UIS system API Error Calls :
            // ErrorCode. => UisErrorCode.TokenWasNotFound,
            // ErrorCode. => UisErrorCode.ParametersMismatch,
            // ErrorCode. => UisErrorCode.IntegratorUrlError,
            // ErrorCode. => UisErrorCode.DatabaseError,
            // ErrorCode. => UisErrorCode.IntegratorUrlDoesNotHaveMapping,
            // ErrorCode. => UisErrorCode.IntegratorServerError,
            // ErrorCode. => UisErrorCode.InvalidToken,
            // ErrorCode. => UisErrorCode.SessionExpired,
            // ErrorCode. => UisErrorCode.InvalidStatusTableGameReading,
            // ErrorCode. => UisErrorCode.TableGameStatusDoesNotExist,
            // ErrorCode. => UisErrorCode.LateBetsRejection,
            // ErrorCode. => UisErrorCode.TableGameIsInClosingProcedure,
            // ErrorCode. => UisErrorCode.TableGameIsClosedNotAvailable,
            // ErrorCode. => UisErrorCode.NoProperBetsReported,
            // ErrorCode. => UisErrorCode.InsufficientFundsAtStpSystem,
            // ErrorCode. => UisErrorCode.PlayerRecordIsLockedForTooLong,
            // ErrorCode. => UisErrorCode.PlayerBalanceUpdateError,
            // ErrorCode. => UisErrorCode.IntegratorPlayerOperatorHasBeenChanged,
            // ErrorCode. => UisErrorCode.IntegrationErrorUnableToBuildIntegratorPlayerInHostSystem,
            // ErrorCode. => UisErrorCode.InternalDbErrorCouldNotLocateBuiltPlayerId,
            // ErrorCode. => UisErrorCode.InternalDbErrorFailToInsertIntegratorToMappingTable,
            // ErrorCode. => UisErrorCode.InvalidTableGameId,
            // ErrorCode. => UisErrorCode.IntegrationBetErrorIntegrator,
            // ErrorCode. => UisErrorCode.InsufficientFundsAtIntegratorSystem,
            // ErrorCode. => UisErrorCode.PermissionDenied,
            // ErrorCode. => UisErrorCode.IntegratorHasPastFaultThatNeedsAttentionPleaseContactYourSupplierFailSafetySystem
        };
    }
}
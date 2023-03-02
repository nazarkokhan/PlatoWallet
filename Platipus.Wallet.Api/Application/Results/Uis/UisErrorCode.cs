namespace Platipus.Wallet.Api.Application.Results.Uis;

public enum UisErrorCode
{
    InsufficientFunds = 300,
    OperationFailed = 301,
    UnknownTransactionIdOrWasAlreadyProcessed = 302,
    UnknownUserId = 310,
    InternalError = 399,
    InvalidToken = 400,
    InvalidHash = 500

    // TokenWasNotFound = 2,
    // ParametersMismatch = 3,
    // IntegratorUrlError = 5,
    // DatabaseError = 29,
    // IntegratorUrlDoesNotHaveMapping = 55,
    // IntegratorServerError = 56,
    // InvalidToken = 101,
    // SessionExpired = 102,
    // InvalidStatusTableGameReading = 103,
    // TableGameStatusDoesNotExist = 104,
    // LateBetsRejection = 105,
    // TableGameIsInClosingProcedure = 106,
    // TableGameIsClosedNotAvailable = 107,
    // NoProperBetsReported = 108,
    // InsufficientFundsAtStpSystem = 109,
    // PlayerRecordIsLockedForTooLong = 110,
    // PlayerBalanceUpdateError = 111,
    // IntegratorPlayerOperatorHasBeenChanged = 137,
    // IntegrationErrorUnableToBuildIntegratorPlayerInHostSystem = 138,
    // InternalDbErrorCouldNotLocateBuiltPlayerId = 141,
    // InternalDbErrorFailToInsertIntegratorToMappingTable = 142,
    // InvalidTableGameId = 155,
    // // PlayerRecordIsLockedForTooLong = 175,
    // IntegrationBetErrorIntegrator = 200,
    // InsufficientFundsAtIntegratorSystem = 212,
    // PermissionDenied = 222,
    // IntegratorHasPastFaultThatNeedsAttentionPleaseContactYourSupplierFailSafetySystem = 555
}
namespace Platipus.Wallet.Api.Application.Results.Betflag;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum BetflagErrorCode
{
    SUCCSESS = 0,
    InvalidToken = 1,
    BetFail2 = 2,
    InsufficientFunds = 3,
    WalletNotFound = 4,
    InvalidParameter = 5,
    BetNotFound = 6,
    ProviderOnMaintenance = 7,
    SessionExpired = 8,
    ClientBlocked = 9,
    BetWithCancel = 10,
    WinWithoutBet = 20,
    WinWithBetError = 21,
    WinWithBetCancelled = 22,
    CancelReferBetNotExists = 30,
    CancelIncorrectAmount = 31,
    CancelReferBetInTimeout = 32,
    CancelReferBetInError = 33,
    CancelWithWin = 34,
    RoundEndBetNotExists = 40,
    RoundEndBetInError = 41,
    SessionCloseInconsistentValues = 95,
    SessionOpenErrorOpenTicket = 96,
    WalletException = 97,
    TemporanyError = 99,
    Exception = 254,
    GeneralError = 255
}
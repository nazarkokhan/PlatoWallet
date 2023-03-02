namespace Platipus.Wallet.Api.Application.Results.Reevo;

public enum ReevoErrorCode
{
    Success = 200,
    BetRefused = 403,
    RollbackRefused = 404,
    InternalError = 500
}
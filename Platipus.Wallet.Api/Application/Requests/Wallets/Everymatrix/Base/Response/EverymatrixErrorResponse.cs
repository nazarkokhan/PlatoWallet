namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using Results.Everymatrix;

public record EverymatrixErrorResponse(string Status, string ErrorCode, string Description, string LogId = "0");
namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

using Results.Everymatrix;

public record EverymatrixErrorResponse(EverymatrixErrorCode Status);
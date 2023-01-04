namespace Platipus.Wallet.Api.Application.Requests.Wallets.Everymatrix.Base.Response;

public record EveryMatrixBaseResponse(
    string Status,
    decimal TotalBalance,
    string Currency);
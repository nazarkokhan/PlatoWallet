namespace Platipus.Wallet.Api.Application.Responses.Vegangster;

using Base;

public sealed record VegangsterTransactionResponse(
    string Status,
    string Currency,
    int Balance,
    string ExternalTransactionId) : VegangsterCommonResponse(
    Status,
    Currency,
    Balance);
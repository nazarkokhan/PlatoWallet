namespace Platipus.Wallet.Api.Application.Responses.Vegangster;

using Base;

public sealed record VegangsterPlayerBalanceResponse(string Status, string Currency, int Balance) : VegangsterCommonResponse(
    Status,
    Currency,
    Balance);
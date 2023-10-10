namespace Platipus.Wallet.Api.Application.Responses.Vegangster;

using Base;

public sealed record VegangsterPlayerBalanceResponse(string Status, string Currency, long Balance) : VegangsterCommonResponse(
    Status,
    Currency,
    Balance);
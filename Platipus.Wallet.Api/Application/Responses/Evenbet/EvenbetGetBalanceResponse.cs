namespace Platipus.Wallet.Api.Application.Responses.Evenbet;

using Base;

public sealed record EvenbetGetBalanceResponse(
        decimal Balance,
        string Timestamp)
    : EvenbetCommonResponse(Balance, Timestamp);
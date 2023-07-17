namespace Platipus.Wallet.Api.Application.Responses.Evenbet;

using Base;

public sealed record EvenbetGetBalanceResponse(
        int Balance,
        string Timestamp)
    : EvenbetCommonResponse(Balance, Timestamp);
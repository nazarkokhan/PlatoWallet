namespace Platipus.Wallet.Api.Application.Responses.Evenbet;

using Base;

public sealed record EvenbetDebitResponse(
    int Balance,
    string Timestamp,
    string TransactionId) : EvenbetCommonResponseWithTransaction(Balance, Timestamp, TransactionId);
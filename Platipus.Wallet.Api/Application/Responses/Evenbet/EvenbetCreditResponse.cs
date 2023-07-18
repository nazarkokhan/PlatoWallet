namespace Platipus.Wallet.Api.Application.Responses.Evenbet;

using Base;

public sealed record EvenbetCreditResponse(
    int Balance,
    string Timestamp,
    string TransactionId) : EvenbetCommonResponseWithTransaction(Balance, Timestamp, TransactionId);
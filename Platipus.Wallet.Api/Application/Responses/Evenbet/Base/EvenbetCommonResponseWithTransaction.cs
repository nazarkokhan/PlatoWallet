namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using Newtonsoft.Json;

public abstract record EvenbetCommonResponseWithTransaction(
    int Balance,
    string Timestamp,
    [property: JsonProperty("transactionId")] string TransactionId) : EvenbetCommonResponse(Balance, Timestamp);
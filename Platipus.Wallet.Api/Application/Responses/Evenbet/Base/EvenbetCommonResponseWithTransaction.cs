namespace Platipus.Wallet.Api.Application.Responses.Evenbet.Base;

using Newtonsoft.Json;

public abstract record EvenbetCommonResponseWithTransaction(
    decimal Balance,
    string Timestamp,
    [property: JsonProperty("transactionId")] string TransactionId) : EvenbetCommonResponse(Balance, Timestamp);
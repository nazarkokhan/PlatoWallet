namespace Platipus.Wallet.Api.Application.Requests.Wallets.Synot.Models;

using System.Text.Json.Serialization;

public sealed record SynotTransactionDetail(
    [property: JsonPropertyName("scratchCardTicketNumber")] string ScratchCardTicketNumber,
    [property: JsonPropertyName("scratchCardEmissionId")] string ScratchCardEmissionId);
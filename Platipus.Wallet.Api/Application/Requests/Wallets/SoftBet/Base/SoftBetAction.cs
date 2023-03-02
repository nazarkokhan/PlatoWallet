namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base;

using System.Text.Json.Nodes;

public record SoftBetAction(string Command, JsonNode? Parameters);
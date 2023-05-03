namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base;

using System.Text.Json;

public record SoftBetAction(string Command, JsonDocument? Parameters); //TODO jsonNode bad logging
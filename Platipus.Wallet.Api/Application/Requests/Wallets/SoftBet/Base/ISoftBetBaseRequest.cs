namespace Platipus.Wallet.Api.Application.Requests.Wallets.SoftBet.Base;

using System.Text.Json.Nodes;
using Requests.Base;

public record SoftBetSingleRequest(
    string ProviderGameId,
    int LicenseeId,
    string Token,
    Guid SessionId,
    string PlayerId,
    string Username,
    string Currency,
    int IsbSkinId,
    int IsbGameId,
    string State,
    string Operator,
    SoftBetAction Action) : IBaseWalletRequest;

public record SoftBetAction(string Command, JsonNode? Parameters);
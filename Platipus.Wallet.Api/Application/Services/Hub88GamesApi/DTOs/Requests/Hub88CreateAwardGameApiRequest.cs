namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

using System.Text.Json.Serialization;
using StartupSettings.JsonConverters;

public record Hub88CreateAwardGameApiRequest(
    string RewardUuid,
    string Currency,
    string User,
    string SubPartnerId,
    [property: JsonConverter(typeof(JsonCustomDateTimeConverter))] DateTime StartTime,
    string PrepaidUuid,
    string GameCode,
    string OperatorId,
    int GameId,
    [property: JsonConverter(typeof(JsonCustomDateTimeConverter))] DateTime EndTime,
    int BetValue,
    int BetCount);
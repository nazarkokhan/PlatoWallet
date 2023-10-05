namespace Platipus.Wallet.Api.Application.Services.UisGamesApi.Dto;

using System.Text.Json.Serialization;

public sealed record UisGetLaunchGameApiRequest(
    string Token,
    [property: JsonPropertyName("operatorId")] int OperatorId,
    [property: JsonPropertyName("launchType")] LaunchMode LaunchType);
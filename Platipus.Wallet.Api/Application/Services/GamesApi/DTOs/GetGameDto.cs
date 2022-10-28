namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs;

public record GetGameDto(
    string Id,
    string GameId,
    string Name,
    string Category);
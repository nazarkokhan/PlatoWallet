namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs;

public record GetPswGameDto(
    string Id,
    string GameId,
    string Name,
    string Category);
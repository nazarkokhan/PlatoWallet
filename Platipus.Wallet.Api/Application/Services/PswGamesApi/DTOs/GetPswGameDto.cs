namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs;

public record GetPswGameDto(
    string Id,
    string GameId,
    string Name,
    string Category);
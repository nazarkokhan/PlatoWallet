namespace PlatipusWallet.Api.Application.Services.GamesApiService.DTOs;

public record GetGameDto(
    string Id,
    string GameId,
    string Name,
    string Category);
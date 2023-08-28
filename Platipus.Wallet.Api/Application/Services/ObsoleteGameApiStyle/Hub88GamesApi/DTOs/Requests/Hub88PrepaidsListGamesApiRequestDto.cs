namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;

public record Hub88PrepaidsListGamesApiRequestDto(
    string? OperatorId,
    int? GameId,
    string? GameCode);
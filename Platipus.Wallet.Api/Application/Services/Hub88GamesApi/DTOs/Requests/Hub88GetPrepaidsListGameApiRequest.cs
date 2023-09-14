namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GetPrepaidsListGameApiRequest(
    string OperatorId,
    int? GameId,
    string GameCode);
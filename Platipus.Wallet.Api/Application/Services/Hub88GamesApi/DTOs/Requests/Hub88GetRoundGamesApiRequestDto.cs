namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GetRoundGamesApiRequestDto(
    string User,
    string TransactionUuid,
    string Round,
    string OperatorId);
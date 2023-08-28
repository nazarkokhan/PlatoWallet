namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.Hub88GamesApi.DTOs.Requests;

public record Hub88GetRoundGamesApiRequestDto(
    string User,
    string TransactionUuid,
    string Round,
    string OperatorId);
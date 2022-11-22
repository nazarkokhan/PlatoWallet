namespace Platipus.Wallet.Api.Application.Services.Hub88GamesApi.DTOs.Requests;

public record Hub88GetGameLinkGamesApiRequestDto(
    string User,
    string Token,
    string SubPartnerId,
    string Platform,
    string OperatorId,
    Hub88GameServerMetaDto Meta,
    string LobbyUrl,
    string Lang,
    string Ip,
    int GameId,
    string GameCode,
    string DepositUrl,
    string Currency,
    string Country);
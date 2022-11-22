namespace Platipus.Wallet.Api.Application.Requests.DTOs;

public record GetCommonGameDto(
    int Id,
    int GameServerId,
    string Name,
    string LaunchName,
    int Category);
namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs.Base;

using Requests.Wallets.Psw.Base.Response;
using Results.Psw;

public record PswBaseGamesApiErrorResponseDto(
    PswStatus Status,
    PswErrorCode Error,
    string Description) : PswBaseResponse(Status);
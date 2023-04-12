namespace Platipus.Wallet.Api.Application.Services.PswGamesApi.DTOs.Responses;

using Requests.Wallets.Psw.Base.Response;
using Results.Psw;

public record PswGamesApiErrorResponseDto(
    PswStatus Status,
    PswErrorCode Error,
    string Description) : PswBaseResponse(Status);
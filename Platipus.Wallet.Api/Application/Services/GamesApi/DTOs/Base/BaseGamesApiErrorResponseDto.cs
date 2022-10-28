namespace Platipus.Wallet.Api.Application.Services.GamesApi.DTOs.Base;

using Results.Common;
using Requests.Base.Responses;
using Results.External.Enums;

public record BaseGamesApiErrorResponseDto(
    Status Status,
    ErrorCode Error,
    string Description) : BaseResponse(Status);
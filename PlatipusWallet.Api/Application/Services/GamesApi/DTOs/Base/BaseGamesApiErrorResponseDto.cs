namespace PlatipusWallet.Api.Application.Services.GamesApi.DTOs.Base;

using PlatipusWallet.Api.Application.Requests.Base.Responses;
using PlatipusWallet.Api.Results.Common;
using PlatipusWallet.Api.Results.External.Enums;

public record BaseGamesApiErrorResponseDto(
    Status Status,
    ErrorCode Error,
    string Description) : BaseResponse(Status);
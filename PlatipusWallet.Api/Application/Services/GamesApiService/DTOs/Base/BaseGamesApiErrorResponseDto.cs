namespace PlatipusWallet.Api.Application.Services.GamesApiService.DTOs.Base;

using PlatipusWallet.Api.Application.Requests.Base.Responses;
using Results.Common;
using PlatipusWallet.Api.Results.External.Enums;

public record BaseGamesApiErrorResponseDto(
    Status Status,
    ErrorCode Error,
    string Description) : BaseResponse(Status);
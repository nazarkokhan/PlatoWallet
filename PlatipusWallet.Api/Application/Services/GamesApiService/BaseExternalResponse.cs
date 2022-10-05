namespace PlatipusWallet.Api.Application.Services.GamesApiService;

using Requests.Base.Responses;
using Results.Common;
using Results.External.Enums;

public record BaseExternalResponse(
    Status Status,
    ErrorCode Error,
    string Description) : BaseResponse(Status);
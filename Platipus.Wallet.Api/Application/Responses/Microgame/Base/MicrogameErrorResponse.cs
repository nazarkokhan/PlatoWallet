namespace Platipus.Wallet.Api.Application.Responses.Microgame.Base;

using Results.Microgame;

public sealed record MicrogameErrorResponse(
    MicrogameStatusCode StatusCode, 
    string StatusMessage) : MicrogameCommonResponse(
    StatusCode);
namespace Platipus.Wallet.Api.Application.Services.SwGameApi.Responses;

public record SwErrorEngineGameApiResponse(
    int StatusCode,
    string Error,
    int Sw,
    string Message);
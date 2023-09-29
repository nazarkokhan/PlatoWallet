namespace Platipus.Wallet.Api.Application.Services.SoftBetGameApi.Responses;

public sealed record SoftBetGsFailureResponse(
    string Error, string Description, string Status = "error");
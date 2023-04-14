namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

using Hub88GamesApi.DTOs.Requests;

public record SoftswissGetLaunchUrlGameApiRequest(
    string CasinoId,
    int Game,
    string Currency,
    string Locale,
    string Ip,
    long? Balance,
    string ClientType,
    SoftswissGamesApiUrls Urls,
    // string Jurisdiction,
    SoftswissGamesApiUser User) : IPswGamesApiBaseRequest;
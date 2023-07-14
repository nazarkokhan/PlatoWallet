namespace Platipus.Wallet.Api.Application.Services.SoftswissGamesApi.DTOs.Requests;

using PswGamesApi.DTOs.Requests;

public record SoftswissGetLaunchUrlGameApiRequest(
    string CasinoId,
    int Game,
    string Currency,
    string Locale,
    string Ip,
    long? Balance,
    string ClientType,
    SoftswissGamesApiUrls Urls,
    SoftswissGamesApiUser User) : IPswGamesApiBaseRequest;
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
    string Jurisdiction,
    SoftswissGamesApiUser User) : IPswGamesApiBaseRequest;

public record SoftswissGamesApiUrls(
    string DepositUrl,
    string ReturnUrl);

public record SoftswissGamesApiUser(
    string Id,
    string Nickname,
    string Firstname = "",
    string Lastname = "",
    string City = "Moscow",
    string Country = "UA",
    string DateOfBirth = "1980-12-26",
    string Gender = "m",
    string RegisteredAt = "2018-10-11");

public record SoftswissIssueFreespinsGameApiRequest(
    string CasinoId,
    string Currency,
    string IssueId,
    string[] Games,
    int FreespinsQuantity,
    int BetLevel,
    DateTime ValidUntil,
    SoftswissGamesApiUser User) : IPswGamesApiBaseRequest;

public record SoftswissCancelFreespinsGameApiRequest(string CasinoId, string IssueId) : IPswGamesApiBaseRequest;

public record SoftswissRoundDetailsGameApiRequest(
    string CasinoId,
    string RoundId,
    string RoundType,
    string Provider) : IPswGamesApiBaseRequest;

public record SoftswissRoundDetailsGameApiResponse(
    string ProviderIdentifier,
    string CasinoIdentifier,
    string UserId,
    bool? Finished);
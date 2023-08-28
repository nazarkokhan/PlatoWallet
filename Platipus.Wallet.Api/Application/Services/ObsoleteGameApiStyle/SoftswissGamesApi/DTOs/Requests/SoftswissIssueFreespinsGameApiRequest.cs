namespace Platipus.Wallet.Api.Application.Services.ObsoleteGameApiStyle.SoftswissGamesApi.DTOs.Requests;

using System.ComponentModel;

public record SoftswissIssueFreespinsGameApiRequest(
    [property: DefaultValue("softswiss")] string CasinoId,
    [property: DefaultValue("USD")] string Currency,
    [property: DefaultValue("123hfksdnf1e")] string IssueId,
    [property: DefaultValue(new[] { "568", "123" })] string[] Games,
    [property: DefaultValue(5)] int FreespinsQuantity,
    [property: DefaultValue(1)] int BetLevel,
    DateTime ValidUntil,
    SoftswissGamesApiUser User) : ISoftswissGameApiBaseRequest;
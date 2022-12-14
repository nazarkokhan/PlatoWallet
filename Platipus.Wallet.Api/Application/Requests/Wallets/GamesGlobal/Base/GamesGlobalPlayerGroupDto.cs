namespace Platipus.Wallet.Api.Application.Requests.Wallets.GamesGlobal.Base;

using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8618
public record GamesGlobalPlayerGroupDto
{
    public string PlayerGroupName { get; init; }
    public int Rank { get; init; }
    public string PlayerGroupId { get; init; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalFreeGameDetailsDto
{
    public int offerId { get; init; }
    public string offerName { get; init; }
    public string balanceType { get; init; }
    public string bundledGames { get; init; }
    public string offerInstances { get; init; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalBundledGamesDto
{
    public int numberOfCoins { get; init; }
    public int numberOfPaylines { get; init; }
    public int chipSize { get; init; }
    public long costPerBet { get; init; }
    public string FriendlyGameName { get; init; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalOfferInstancesDto
{
    public int instanceId { get; init; }
    public int numberOfGamesAwarded { get; init; }
    public int numberOfGamesRemaining { get; init; }
    public string offerExpirationDate { get; init; }
    public long totalWinnings { get; init; }
}
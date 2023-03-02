namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal.Base;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalOfferInstancesDto
{
    public int instanceId { get; init; }
    public int numberOfGamesAwarded { get; init; }
    public int numberOfGamesRemaining { get; init; }
    public string offerExpirationDate { get; init; } = null!;
    public long totalWinnings { get; init; }
}
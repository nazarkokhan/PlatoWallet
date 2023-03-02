namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal.Base;

using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public record GamesGlobalBundledGamesDto
{
    public int numberOfCoins { get; init; }
    public int numberOfPaylines { get; init; }
    public int chipSize { get; init; }
    public long costPerBet { get; init; }
    public string FriendlyGameName { get; init; } = null!;
}
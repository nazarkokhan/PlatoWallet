namespace Platipus.Wallet.Api.Application.Requests.Wallets.TODO.GamesGlobal.Base;

#pragma warning disable CS8618
public record GamesGlobalPlayerGroupDto
{
    public string PlayerGroupName { get; init; }
    public int Rank { get; init; }
    public string PlayerGroupId { get; init; }
}
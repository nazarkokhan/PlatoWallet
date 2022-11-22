namespace Platipus.Wallet.Domain.Entities;

using Abstract;

public class CasinoGames : Entity
{
    public string CasinoId { get; set; } = null!;
    public Casino Casino { get; set; } = null!;

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;
}
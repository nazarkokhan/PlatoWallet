namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Game : Entity<int>
{
    public int GameServiceId { get; set; }

    public string Name { get; set; } = null!;

    public string LaunchName { get; set; } = null!;

    public int CategoryId { get; set; }

    public List<CasinoGames> CasinoGames { get; set; } = new();
}
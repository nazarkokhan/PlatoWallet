namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Game : Entity<int>
{
    public int GameServerId { get; set; }

    public string Name { get; set; } = null!;

    public string LaunchName { get; set; } = null!;

    public int CategoryId { get; set; }

    public List<CasinoGames> CasinoGames { get; set; } = new();
}

public class GameServerEnvironmentInfo : Entity<int>
{
    public string Name { get; set; } = null!;

    public string BaseUrl { get; set; } = null!;

    public string LaunchName { get; set; } = null!;

    public int CategoryId { get; set; }

    public List<CasinoGames> CasinoGames { get; set; } = new();
}
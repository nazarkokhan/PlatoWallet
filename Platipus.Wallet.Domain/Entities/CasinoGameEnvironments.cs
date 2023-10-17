namespace Platipus.Wallet.Domain.Entities;

using Abstract;

public class CasinoGameEnvironments : Entity
{
    public string CasinoId { get; set; } = null!;
    public Casino Casino { get; set; } = null!;

    public string GameEnvironmentId { get; set; } = null!;
    public GameEnvironment GameEnvironment { get; set; } = null!;
}
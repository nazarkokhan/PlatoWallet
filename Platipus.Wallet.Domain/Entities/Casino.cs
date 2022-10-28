namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;
using Enums;

public class Casino : Entity<string>
{
    public string SignatureKey { get; set; } = null!;

    public CasinoProvider? Provider { get; set; }
    
    public List<User> Users { get; set; } = new();
    
    public List<CasinoCurrencies> CasinoCurrencies { get; set; } = new();
}
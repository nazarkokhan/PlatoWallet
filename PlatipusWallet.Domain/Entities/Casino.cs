namespace PlatipusWallet.Domain.Entities;

using Abstract;
using Abstract.Generic;

public class Casino : Entity<string>
{
    public string SignatureKey { get; set; } = null!;
    
    public List<User> Users { get; set; } = new();
}

public class CasinoCurrencies : Entity
{
    public string CasinoId { get; set; }
    public Casino Casino { get; set; } = null!;
    
    // public Guid Currency { get; set; }
    // public Casino Casino { get; set; } = null!;
}

public class Currency : Entity
{
    public string Name { get; set; } = null!;
    
    public List<User> Users { get; set; } = new();
}
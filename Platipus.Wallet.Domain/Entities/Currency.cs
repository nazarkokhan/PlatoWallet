namespace Platipus.Wallet.Domain.Entities;

using Abstract;

public class Currency : Entity
{
    public string Name { get; set; } = null!;

    // public CurrencyMetadata CurrencyMetadata { get; set; }
    
    public List<User> Users { get; set; } = new();
    
    public List<CasinoCurrencies> CasinoCurrencies { get; set; } = new();
}
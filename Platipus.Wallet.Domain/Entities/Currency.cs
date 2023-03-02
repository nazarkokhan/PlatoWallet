namespace Platipus.Wallet.Domain.Entities;

using Abstract.Generic;

public class Currency : Entity<string>
{
    public Currency(string id)
    {
        Id = id;
    }

    public List<User> Users { get; set; } = new();

    public List<CasinoCurrencies> CasinoCurrencies { get; set; } = new();
}
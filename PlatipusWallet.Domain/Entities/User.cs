namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class User : Entity
{
    public decimal Balance { get; set; }
    
    public string Currency { get; set; } = null!;

    public List<Round> Rounds { get; set; } = new();
}
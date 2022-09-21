namespace PlatipusWallet.Domain.Entities;

using Abstract;

public class Round : Entity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public List<Transaction> Transactions { get; set; } = new();
}
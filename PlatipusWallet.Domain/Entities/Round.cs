namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;

public class Round : Entity<string>
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public List<Transaction> Transactions { get; set; } = new();
}
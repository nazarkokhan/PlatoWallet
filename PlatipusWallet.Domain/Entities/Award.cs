namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;

public class Award : Entity<string>
{
    public decimal Amount { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
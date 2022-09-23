namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;

public class Award : Entity<string>
{
    public decimal Amount { get; set; }
    
    public string UserId { get; set; } = null!;
    public User User { get; set; } = null!;
}
namespace PlatipusWallet.Domain.Entities;

using Abstract.Generic;

public class Transaction : Entity<string>
{
    public decimal Amount { get; set; }
    
    public string RoundId { get; set; } = null!;
    public Round Round { get; set; } = null!;
}